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
                RunningProcesses.ForEach(x => body += $"{x.ItemName},");


                return "{ " + body.TrimEnd(',') + " }";
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

        int _blacklistCheckInterval { get; set; } = 5;

        public ProcessWatcher()
        {
            Instance = this;

            _blacklistedProcesses = Bootstrapper.Settings.Mining.BlacklistedProcesses ?? new List<BlacklistedItem>();

            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, _blacklistCheckInterval);
            _timer.Tick += (s, e) => ProcessWatcherTimer_Tick();
            _timer.Start();
        }

        async Task<List<BlacklistedItem>> GetRunningBlacklistedProcesses()
        {
            var procs = new List<BlacklistedItem>();

            foreach (var proc in _blacklistedProcesses)
            {
                var runningProcs = Process.GetProcessesByName(proc.NameWithoutExtension);
                if (runningProcs.Any())
                {
                    procs.Add(proc);
                }
            }

            return procs;
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
    }
}
