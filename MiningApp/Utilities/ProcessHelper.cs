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

namespace MiningApp
{
    public delegate void BlacklistedProcessDelegate(BlacklistedProcessArgs args);


    public class BlacklistedProcessArgs
    {
        public DateTime Timestamp { get; set; }

        public bool BlacklistedProcsRunning { get; set; }

        public List<BlacklistedProcess> RunningProcesses { get; set; }

        public BlacklistedProcessArgs(List<BlacklistedProcess> procs = null)
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
    }

    public class ProcessHelper
    {
        private List<SessionConfigModel> _allMiners = new List<SessionConfigModel>();
        private List<Process> _minerProcesses = new List<Process>();

        public ProcessHelper()
        {
            UpdateMiners();
            GetRunningMiners();
        }

        private async void UpdateMiners()
        {
            //_allMiners = await _controller.GetMiners();
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

        public void StartMiner(SessionConfigModel miner)
        {
            
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

        List<BlacklistedProcess> _blacklistedProcesses { get; set; }

        DispatcherTimer _timer { get; set; }

        int _blacklistCheckInterval { get; set; } = 5;

        public ProcessWatcher()
        {
            Instance = this;

            _blacklistedProcesses = Bootstrapper.Settings.Mining.BlacklistedProcesses ?? new List<BlacklistedProcess>();

            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, _blacklistCheckInterval);
            _timer.Tick += (s, e) => ProcessWatcherTimer_Tick();
            _timer.Start();
        }

        async Task<List<BlacklistedProcess>> GetRunningBlacklistedProcesses()
        {
            var procs = new List<BlacklistedProcess>();

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

    public class BlacklistedProcess
    {
        public string ProcessPath { get; set; }

        public string ProcessFileName { get; set; }

        [BsonIgnore]
        public string NameWithoutExtension => ProcessHelper.GetProcessNameFromFile(ProcessFileName);

        public BlacklistedProcess()
        {

        }

        public BlacklistedProcess(string path)
        {
            var fileInfo = new FileInfo(path);

            ProcessPath = fileInfo.FullName;
            ProcessFileName = fileInfo.Name;
        }

        public override string ToString()
        {
            return ProcessFileName;
        }
    }
}
