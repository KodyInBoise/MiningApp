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
            public string GetAllOutput() => _allOutput;

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

        public DateTime LastOutputTimestamp { get; set; }

        public event SessionStatusToggledDelegate StatusToggled;

        public event OutputReceivedDelegate OutputReceived;

        public SessionStatusEnum CurrentStatus { get; set; } = SessionStatusEnum.Stopped;

        public TimerModel Timer { get; set; }

        public TimeSpan Uptime { get; set; }

        public string UptimeString => GetUptimeString();


        bool _redirectOutput { get; set; } = true;

        public SessionModel(SessionConfigModel config, bool start = false)
        {
            Timer = new TimerModel(this);
            Timer.Delegate += SessionTimer_Ticked;

            OutputHelper = new OutputHelperModel(this);

            Config = config;
            Config.Session = this;
            SessionID = Guid.NewGuid().ToString().Substring(0, 8);
        }

        void SessionTimer_Ticked(TimerTickedArgs args)
        {
            Uptime = args.Uptime;
        }

        public async void ToggleStatus(SessionStatusEnum newStatus, string message = "")
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
                        statusMessage = !String.IsNullOrEmpty(message) ? statusMessage += message : statusMessage += "Session Stopped!";
                        OutputHelper.AppendOutput(Environment.NewLine + statusMessage + Environment.NewLine);
                        await Stop(true);
                        break;
                    case SessionStatusEnum.Running:
                        statusMessage = !String.IsNullOrEmpty(message) ? statusMessage += message : statusMessage += "Session Started!";
                        OutputHelper.AppendOutput(Environment.NewLine + statusMessage + Environment.NewLine);
                        Start();
                        break;
                    case SessionStatusEnum.ManuallyPaused:
                        statusMessage = !String.IsNullOrEmpty(message) ? statusMessage += message : statusMessage += "Session Manually Paused!";
                        OutputHelper.AppendOutput(Environment.NewLine + statusMessage + Environment.NewLine);
                        await Stop();
                        break;
                    case SessionStatusEnum.BlacklistPaused:
                        statusMessage = !String.IsNullOrEmpty(message) ? statusMessage += message : statusMessage += "Session Paused Due To Blacklist Processes Running!";
                        OutputHelper.AppendOutput(Environment.NewLine + statusMessage + Environment.NewLine);
                        await Stop();
                        break;
                    default:
                        break;
                }

                CurrentStatus = newStatus;
                StatusToggled?.Invoke(args);

                LogHelper.AddEntry(LogType.Session, statusMessage);

                if (Bootstrapper.Settings.Server.UseServer)
                {
                    await Task.Run(() => ServerHelper.UpdateClientConfig(Config, LocalClientModel.Instance.ID));
                }
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

        public async Task Stop(bool clearSession = false)
        {
            try
            {
                if (clearSession)
                {
                    Timer.ToggleStatus(TimerAction.Stop);
                    WindowController.MiningSessions.Remove(this);
                }

                var procs = Process.GetProcessesByName(MinerProcess.ProcessName).ToList();
                procs.ForEach(x => x.Kill());
            }
            catch (InvalidOperationException) { } // Swallow invalid operation exceptions when process is already killed
            catch (Exception ex)
            {
                LogHelper.AddEntry(ex);
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
            return Timer.GetTimer;
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

        public string GetUptimeString()
        {
            if (Uptime.Days > 0)
            {
                return $"{Uptime.Days} Days {Uptime.Hours} Hours {Uptime.Minutes} Minutes {Uptime.Seconds}Seconds";
            }
            else if (Uptime.Hours > 0)
            {
                return $"{Uptime.Hours} Hours {Uptime.Minutes} Minutes {Uptime.Seconds} Seconds";
            }
            else if (Uptime.Minutes > 0)
            {
                return $"{Uptime.Minutes} Minutes {Uptime.Seconds} Seconds";
            }
            else if (Uptime.Seconds > 0)
            {
                return $"{Uptime.Seconds} Seconds";
            }

            return string.Empty;
        }
    }
}
