using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Management;
using System.Windows.Threading;
using LiteDB;
using Newtonsoft.Json;
using MiningApp.LoggingUtil;

namespace MiningApp
{
    public delegate void BlacklistedProcessDelegate(BlacklistedProcessArgs args);


    public class BlacklistedProcessArgs
    {
        public DateTime Timestamp { get; set; }

        public bool BlacklistedProcsRunning { get; set; }

        public List<BlacklistedItem> RunningProcesses { get; set; }

        public string StatusMessage => GetProcessString();

        public BlacklistedProcessArgs(List<BlacklistedItem> procs = null)
        {
            Timestamp = DateTime.Now;
            
            if (procs?.Count > 0)
            {
                BlacklistedProcsRunning = true;
                RunningProcesses = procs;
            }
            else
            {
                BlacklistedProcsRunning = false;
            }
        }

        string GetProcessString()
        {
            var body = "";

            if (BlacklistedProcsRunning)
            {
                RunningProcesses.ForEach(x => body += $"{x.ItemName}, ");


                return "{ " + body.TrimEnd(new[] { ' ', ',' }) + " }";
            }
            else
            {
                return "No Blacklisted proccesses running...";
            }
        }
    }

    public class ProcessHelper
    {
        private List<SessionConfigModel> _allMiners = new List<SessionConfigModel>();
        private List<Process> _minerProcesses = new List<Process>();

        public ProcessHelper()
        {
            GetRunningMiners();
        }

        private void GetRunningMiners()
        {
            /*
            foreach (var miner in _allMiners)
            {
                var proc = miner.GetProcess();

                if (proc != null)
                {
                    _minerProcesses.Add(proc);
                }
            }
            */
            //Testing
            _minerProcesses.ForEach(x => Console.WriteLine(x.ProcessName));
        }

        public static List<Process> GetChildProcesses(Process proc)
        {
            var childProcesses = new List<Process>();
            

            return childProcesses;
        }

        public static string GetProcessNameFromFile(string filePath)
        {
            try
            {
                var processName = filePath.Substring(0, filePath.LastIndexOf('.'));

                return processName;
            }
            catch
            {
                return "";
            }
        }
    }

    public class ProcessWatcher
    {
        public static ProcessWatcher Instance { get; set; }

        public BlacklistedProcessDelegate BlacklistedProcsDelegate;

        List<BlacklistedItem> _blacklistedProcesses { get; set; }

        DispatcherTimer _timer { get; set; }

        int _blacklistCheckInterval { get; set; } = 15;

        Task<List<BlacklistedItem>> GetBlacklistedProcesses => Bootstrapper.Settings.Mining.GetAllBlacklistedProcesses();

        public ProcessWatcher()
        {
            Instance = this;

            Initialize();
        }

        async void Initialize()
        {
            _blacklistedProcesses = await GetBlacklistedProcesses;

            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, _blacklistCheckInterval);
            _timer.Tick += (s, e) => ProcessWatcherTimer_Tick();
            _timer.Start();
        }

        async Task<List<BlacklistedItem>> GetRunningBlacklistedProcesses()
        {
            var runningProcs = new List<BlacklistedItem>();
            _blacklistedProcesses = await GetBlacklistedProcesses;

            if (_blacklistedProcesses.Any())
            {
                _blacklistedProcesses = _blacklistedProcesses.FindAll(x => !x.ExcludeFromBlacklist);

                foreach (var proc in _blacklistedProcesses)
                {
                    var running = Process.GetProcessesByName(proc.NameWithoutExtension);
                    if (running.Any())
                    {
                        runningProcs.Add(proc);
                    }
                }
            }

            return runningProcs;
        }

        async void CheckForBlacklistedProcesses()
        {
            var runningProcs = await GetRunningBlacklistedProcesses();

            BlacklistedProcsDelegate?.Invoke(new BlacklistedProcessArgs(runningProcs));
        }

        void ProcessWatcherTimer_Tick()
        {
            if (Bootstrapper.Settings.Mining.UseBlackList)
            {
                CheckForBlacklistedProcesses();
            }
        }
    }

    public class BlacklistedItem
    {
        public BlacklistedItemType BlacklistType { get; set; }

        public string FullPath { get; set; }

        public string ItemName => GetItemName();

        [JsonIgnore]
        public bool IsRunning => IsProcessRunning();

        [JsonIgnore]
        public bool ExcludeFromBlacklist { get; set; } = false;

        [JsonIgnore]
        public string NameWithoutExtension => ProcessHelper.GetProcessNameFromFile(ItemName);

        public BlacklistedItem(BlacklistedItemType type, string path)
        {
            BlacklistType = type;
            FullPath = path;
        }

        string GetItemName()
        {
            try
            {
                switch (BlacklistType)
                {
                    case BlacklistedItemType.Executable:
                        var file = new FileInfo(FullPath);
                        return file.Name;
                    case BlacklistedItemType.Directory:
                        var dir = new DirectoryInfo(FullPath);
                        return dir.Name;
                    default:
                        return "empty";
                }
            }
            catch
            {
                return "error";
            }
        }

        public override string ToString()
        {
            switch (BlacklistType)
            {
                case BlacklistedItemType.Executable:
                    return ItemName;
                case BlacklistedItemType.Directory:
                    return FullPath;
                default:
                    return "empty";
            }
        }

        public List<string> GetDirectoryExecutablePaths()
        {
            var exePaths = new List<string>();

            try
            {
                if (BlacklistType == BlacklistedItemType.Executable)
                {
                    return null;
                }

                var allFiles = FileHelper.GetAllDirectoryFiles(FullPath);
                foreach (var file in allFiles)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.Extension.Contains("exe"))
                    {
                        exePaths.Add(fileInfo.FullName);
                    }
                }

                return exePaths;
            }
            catch (UnauthorizedAccessException ex)
            {
                ExcludeFromBlacklist = true;
                LogHelper.AddEntry($"Removed \"{ItemName}\" from Blacklist due to permission issues.");

                return exePaths; 
            }
            catch (Exception ex)
            {
                LogHelper.AddEntry(ex);

                return exePaths;
            }
        }

        bool IsProcessRunning()
        {
            if (BlacklistType == BlacklistedItemType.Directory)
            {
                return false;
            }

            var procs = Process.GetProcessesByName(NameWithoutExtension);
            if (procs.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
