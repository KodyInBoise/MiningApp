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
        public DateTime Timestamp { get; set; }
        public string SessionID { get; set; }
        public SessionStatusEnum NewStatus { get; set; }
    }

    public class OutputReceivedArgs
    {
        public DateTime Timestamp { get; set; }
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
        public string SessionID { get; set; }

        public SessionConfigModel Config { get; set; }

        public MinerConfigModel Miner => Config.Miner;

        public WalletConfigModel Wallet => Config.Wallet;

        public PoolConfigModel Pool => Config.Pool;

        public Process MinerProcess { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime LastOutputTimestamp { get; set; }

        public TimeSpan Uptime => GetUptime();

        public string UptimeString => _sessionTimer.GetUptimeFriendlyString();

        public string NewOutput => GetNewOutput();

        public string AllOutput => _output;

        public event OutputReceivedDelegate OutputReceived;

        public SessionStatusEnum CurrentStatus { get; set; } = SessionStatusEnum.Stopped;

        public event SessionStatusToggledDelegate StatusToggled;

        TimerHelper _sessionTimer { get; set; }

        string _output { get; set; } = "";

        bool _redirectOutput { get; set; } = true;

        public SessionModel(SessionConfigModel config)
        {
            StartTime = DateTime.Now;
            Config = config;

            SessionID = Guid.NewGuid().ToString().Substring(0, 8);

            _sessionTimer = new TimerHelper(this);

            StatusToggled += SessionToggled_Invoked;
        }

        public void Start(bool clearOutput = false)
        {
            try
            {
                if (!IsRunning())
                {
                    StatusToggled?.Invoke(new SessionStatusToggledArgs() { Timestamp = DateTime.Now, NewStatus = SessionStatusEnum.InProgress, SessionID = SessionID });

                    SetMinerSettings();

                    Task.Run(RunMinerProcess);

                    LogHelper.AddEntry(LogType.Session, $"Session started: Config = \"{Config.Name}\"");
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

                AppendOutput(output);
            }
        }

        public void Pause()
        {
            StatusToggled?.Invoke(new SessionStatusToggledArgs() { Timestamp = DateTime.Now, NewStatus = SessionStatusEnum.Stopped, SessionID = SessionID });
            _sessionTimer.StopTimer();

            MinerProcess.Close();
        }

        public async Task Stop()
        {
            StatusToggled?.Invoke(new SessionStatusToggledArgs() { Timestamp = DateTime.Now, NewStatus = SessionStatusEnum.Stopped, SessionID = SessionID });
            _sessionTimer.StopTimer();

            WindowController.MiningSessions.Remove(this);

            MinerProcess.Kill();
        }

        void SessionToggled_Invoked(SessionStatusToggledArgs args)
        {
            CurrentStatus = args.NewStatus;

            switch (args.NewStatus)
            {
                case SessionStatusEnum.Stopped:
                    LogHelper.AddEntry(LogType.Session, $"Session stopped: {Config.Name}");
                    break;
                case SessionStatusEnum.InProgress:
                    LogHelper.AddEntry(LogType.Session, $"Session started: {Config.Name}");
                    break;
                case SessionStatusEnum.Paused:
                    LogHelper.AddEntry(LogType.Session, $"Session paused: {Config.Name}");
                    break;
            }
        }

        private void AppendOutput(string output)
        {
            var outputArgs = new OutputReceivedArgs() { SessionID = SessionID, NewOutput = output };
            OutputReceived?.Invoke(outputArgs);

            _output += output + Environment.NewLine;

            _newOutput = output;
            LastOutputTimestamp = DateTime.Now;

            _sessionTimer.ResetStaleOutput();

            Console.WriteLine(output);
        }

        string _newOutput = string.Empty;
        string GetNewOutput()
        {
            var output = _newOutput;
            _newOutput = string.Empty;

            return output;
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
            return _sessionTimer.GetTimer();
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
            _newOutput = $"\r\r----------------------------\rRestarting Miner {DateTime.Now.ToString()}\r----------------------------\r\r";
            OutputReceived?.Invoke(new OutputReceivedArgs() { SessionID = SessionID, NewOutput = _newOutput });

            var procs = Process.GetProcessesByName(MinerProcess.ProcessName).ToList();

            if (procs.Any())
            {
                procs.ForEach(x => x.Kill());
            }

            Start();

            LogHelper.AddEntry(LogType.Session, $"Restarted session: Config = \"{Config.Name}\"");
        }

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
        }
    }
}
