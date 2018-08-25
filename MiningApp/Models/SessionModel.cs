using MiningApp.LoggingUtil;
using MiningApp.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MiningApp
{
    public class SessionStatusToggledArgs
    {
        public DateTime Timestamp { get; private set; }
        public string SessionID { get; set; }
        public SessionStatusEnum NewStatus { get; set; }
        public string StatusMessage { get; set; }

        public SessionStatusToggledArgs()
        {
            Timestamp = DateTime.Now;
        }
    }

    public class OutputReceivedArgs
    {
        public DateTime Timestamp { get; private set; }
        public string SessionID { get; set; }
        public string NewOutput { get; set; }

        public OutputReceivedArgs()
        {
            Timestamp = DateTime.Now;
        }
    }


    public delegate void SessionStatusToggledDelegate(SessionStatusToggledArgs args);

    public delegate void OutputReceivedDelegate(OutputReceivedArgs args);


    public class SessionModel
    {
        public class OutputHelperModel
        {
            public string GetAllOutput => _allOutput;

            SessionModel _session { get; set; }

            DateTime _firstOutputTimestamp { get; set; }

            DateTime _lastOutputTimestamp { get; set; }

            string _allOutput { get; set; }

            public OutputHelperModel(SessionModel session)
            {
                _session = session;
            }

            public void AppendOutput(string output)
            {
                var args = new OutputReceivedArgs() { SessionID = _session.SessionID, NewOutput = output };
                _session.OutputReceived?.Invoke(args);

                _allOutput += output + Environment.NewLine;

                if (String.IsNullOrEmpty(output))
                {
                    _firstOutputTimestamp = args.Timestamp;
                }
                else
                {
                    _lastOutputTimestamp = args.Timestamp;
                }

                // DEBUGGING
                Console.WriteLine(output);
            }
        }

        public string SessionID { get; set; }

        public OutputHelperModel OutputHelper { get; set; }

        public SessionConfigModel Config { get; set; }

        public MinerConfigModel Miner => Config.Miner;

        public WalletConfigModel Wallet => Config.Wallet;

        public PoolConfigModel Pool => Config.Pool;

        public Process MinerProcess { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime LastOutputTimestamp { get; set; }

        public TimeSpan Uptime => GetUptime();

        //public string UptimeString => _sessionTimer.GetUptimeFriendlyString();

        public event SessionStatusToggledDelegate StatusToggled;

        public event OutputReceivedDelegate OutputReceived;

        public SessionStatusEnum CurrentStatus { get; set; } = SessionStatusEnum.Stopped;


        bool _redirectOutput { get; set; } = true;

        public SessionModel(SessionConfigModel config)
        {
            OutputHelper = new OutputHelperModel(this);

            StartTime = DateTime.Now;
            Config = config;

            SessionID = Guid.NewGuid().ToString().Substring(0, 8);
        }

        public void ToggleStatus(SessionStatusEnum newStatus, string message = "")
        {
            if (newStatus == CurrentStatus)
            {
                return;
            }

            try
            {
                var args = new SessionStatusToggledArgs() { SessionID = SessionID, NewStatus = newStatus, StatusMessage = message };
                var statusMessage = $"{args.Timestamp} | {Config.Name} | ";
                switch (newStatus)
                {
                    case SessionStatusEnum.Stopped:
                        statusMessage = !String.IsNullOrEmpty(message) ? statusMessage += message : statusMessage += "Session Stopped!\r";
                        OutputHelper.AppendOutput(statusMessage);
                        MinerProcess.Kill();
                        break;
                    case SessionStatusEnum.Running:
                        statusMessage = !String.IsNullOrEmpty(message) ? statusMessage += message : statusMessage += "Session Started!\r";
                        OutputHelper.AppendOutput(statusMessage);
                        Start();
                        break;
                    case SessionStatusEnum.ManuallyPaused:
                        statusMessage = !String.IsNullOrEmpty(message) ? statusMessage += message : statusMessage += "Session Manually Paused!\r";
                        OutputHelper.AppendOutput(statusMessage);
                        MinerProcess.Kill();
                        break;
                    case SessionStatusEnum.BlacklistPaused:
                        statusMessage = !String.IsNullOrEmpty(message) ? statusMessage += message : statusMessage += "Session Paused Due To Blacklist Processes Running!\r";
                        OutputHelper.AppendOutput(statusMessage);
                        MinerProcess.Kill();
                        break;
                    default:
                        break;
                }

                StatusToggled?.Invoke(args);

                LogHelper.AddEntry(LogType.Session, statusMessage);
            }
            catch (Exception ex)
            {
                LogHelper.AddEntry(ex);
            }
        }

        public void Start(bool clearOutput = false)
        {
            try
            {
                if (!IsRunning())
                {
                    SetMinerSettings();

                    Task.Run(RunMinerProcess);
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddEntry(ex);
            }
        }

        async Task RunMinerProcess()
        {
            MinerProcess = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = Config.Miner.Path,
                    UseShellExecute = false,

                    RedirectStandardOutput = _redirectOutput,
                    CreateNoWindow = _redirectOutput
                }
            };

            MinerProcess.Start();

            while (!MinerProcess.StandardOutput.EndOfStream)
            {
                var output = MinerProcess.StandardOutput.ReadLine();
                
                if (!String.IsNullOrEmpty(output))
                {
                    OutputHelper.AppendOutput(output);
                }
            }
        }

        /*
        void SessionOutputReceived(string newOutput)
        {
            _newOutput = newOutput;

            var args = new OutputReceivedArgs() { SessionID = SessionID, NewOutput = _newOutput };
            OutputReceived?.Invoke(args);
        }

        void OutputReceived_Invoked(OutputReceivedArgs args)
        {
            LastOutputTimestamp = args.Timestamp;
            _output += args.NewOutput + Environment.NewLine;

            Console.WriteLine(args.NewOutput);
        }

        public void AppendOutput(string output)
        {
            SessionOutputReceived(output);
        }
        */

        public async Task Stop()
        {
            WindowController.MiningSessions.Remove(this);
         
            MinerProcess.Kill();
        }

        private TimeSpan GetUptime()
        {
            try
            {
                return MinerProcess != null ? DateTime.Now.Subtract(MinerProcess.StartTime) : new TimeSpan(0, 0, 0);
            }
            catch
            {
                return new TimeSpan(0, 0, 0);
            }
        }

        private async void SetMinerSettings()
        {
            switch (Config.Miner.Type)
            {
                case MinerType.CCMiner:
                    await MinerSettings.CCMiner.SaveParams(Config.Pool.Address, Config.Wallet.Address);
                    break;
            }
        }

        private bool IsRunning()
        {
            try
            {
                var existingProcs = Process.GetProcessesByName(Config.Miner.ProcessName).ToList();

                if (existingProcs.Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public DispatcherTimer GetTimer()
        {
            return null; //_sessionTimer.GetTimer();
        }

        bool OutputIsStale()
        {
            if (IsRunning())
            {
                if (DateTime.Now > MinerProcess?.StartTime)
                {
                    return DateTime.Now > LastOutputTimestamp.AddMinutes(Config.StaleOutputThreshold);
                }

                return false;
            }

            return false;
        }

        public async Task CheckForStaleMiners()
        {
            if (OutputIsStale())
            {
                RestartMiner();
            }
        }

        public async void RestartMiner()
        {
            var procs = Process.GetProcessesByName(MinerProcess.ProcessName).ToList();

            if (procs.Any())
            {
                procs.ForEach(x => x.Kill());
            }

            Start();

            LogHelper.AddEntry(LogType.Session, $"Restarted session: Config = \"{Config.Name}\"");
        }

        /*
        class TimerHelper
        {
            private SessionModel _session { get; set; }

            private DispatcherTimer _timer { get; set; }

            private TimeSpan _uptime { get; set; }

            private int _uptimeSeconds { get; set; }

            private int _uptimeMinutes { get; set; }

            private int _uptimeHours { get; set; }

            private int _uptimeDays { get; set; }

            private int _staleOutputSeconds { get; set; }

            public TimerHelper(SessionModel session)
            {
                _session = session;

                _timer = new DispatcherTimer();
                _timer.Interval = new TimeSpan(0, 0, 1);
                _timer.Tick += (s, e) => Timer_Tick();

                _timer.Start();

                _staleOutputSeconds = 0;
            }

            private void Timer_Tick()
            {
                _uptimeSeconds++;

                if (_uptimeSeconds >= 60)
                {
                    AddUptime();
                }

                _staleOutputSeconds++;

                Task.Run(_session.CheckForStaleMiners);
            }

            public TimeSpan GetUptime()
            {
                return new TimeSpan(_uptimeHours, _uptimeMinutes, _uptimeSeconds);
            }

            private void AddUptime()
            {
                _uptimeMinutes++;

                _uptimeSeconds = 0;

                if (_uptimeMinutes >= 60)
                {
                    _uptimeHours++;
                    _uptimeMinutes = _uptimeMinutes % 60;
                }
                if (_uptimeHours >= 60)
                {
                    _uptimeDays++;
                    _uptimeHours = _uptimeHours % 60;
                }               
            }

            public DispatcherTimer GetTimer()
            {
                return _timer;
            }

            public string GetUptimeFriendlyString()
            {
                var uptime = "";

                if (_uptimeDays > 0)
                {
                    uptime = $"{_uptimeDays} Days {_uptimeHours} Hours {_uptimeMinutes} Minutes";
                }
                else
                {
                    uptime = $"{_uptimeHours} Hours {_uptimeMinutes} Minutes {_uptimeSeconds} Seconds";
                }

                return uptime;
            }

            public void ResetStaleOutput()
            {
                _staleOutputSeconds = 0;
            }

            public int GetStaleOutputSeconds()
            {
                return _staleOutputSeconds;
            }

            public void StopTimer()
            {
                _timer.Stop();
            }

            public void ResumeTimer()
            {
                _timer.Start();
            }
        }
        */
    }
}
