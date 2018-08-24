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

namespace MiningApp
{
    public delegate void BlacklistedProcessDelegate(BlacklistedProcessArgs args);


    public class BlacklistedProcessArgs
    {
        public DateTime Timestamp { get; set; }

        public bool BlacklistedProcsRunning { get; set; }

        public List<string> ProcessNames { get; set; }

        public BlacklistedProcessArgs(List<string> processNames = null)
        {
            Timestamp = DateTime.Now;
            
            if (processNames?.Count > 0)
            {
                BlacklistedProcsRunning = true;
                ProcessNames = processNames;
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
    }

    public class ProcessWatcher
    {
        public static ProcessWatcher Instance { get; set; }

        public BlacklistedProcessDelegate BlacklistedProcsDelegate;

        List<string> _blacklistedProcesses { get; set; }

        DispatcherTimer _timer { get; set; }

        int _blacklistCheckInterval { get; set; } = 5;

        public ProcessWatcher()
        {
            Instance = this;

            _blacklistedProcesses = Bootstrapper.Settings.Mining.BlacklistedProcesses ?? new List<string>();

            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, _blacklistCheckInterval);
            _timer.Tick += (s, e) => CheckForBlacklistedProcesses();
            _timer.Start();
        }

        bool BlacklistedProcessesRunning()
        {
            var processes = new List<Process>();

            foreach (var process in _blacklistedProcesses)
            {
                var procs = Process.GetProcessesByName(process);
                processes.AddRange(procs);
            }

            if (processes.Any())
            {
                return true;
            }

            return false;
        }

        async Task<List<string>> GetRunningBlacklistedProcNames()
        {
            var processes = new List<string>();

            foreach (var process in _blacklistedProcesses)
            {
                var procs = Process.GetProcessesByName(process).ToList();
                procs.ForEach(x => processes.Add(x.ProcessName));
            }

            return processes;
        }

        async void CheckForBlacklistedProcesses()
        {
            var runningProcs = await GetRunningBlacklistedProcNames();

            BlacklistedProcsDelegate?.Invoke(new BlacklistedProcessArgs(runningProcs));
        }
    }
}
