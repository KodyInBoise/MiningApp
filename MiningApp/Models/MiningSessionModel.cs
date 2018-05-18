using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MiningApp
{
    public class MiningSessionModel
    {
        public ConfigModel Config { get; set; }

        public Process MinerProcess { get; set; }

        public DateTime StartTime { get; set; }

        public TimeSpan Uptime => _sessionTimer.GetUptime();

        
        TimerHelper _sessionTimer { get; set; }



        public MiningSessionModel(ConfigModel config)
        {
            StartTime = DateTime.Now;
            Config = config;

            _sessionTimer = new TimerHelper();
        }

        public void Start()
        {
            if (!IsRunning())
            {
                MinerProcess = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = Config.Miner.Path,
                    }
                };

                SetMinerSettings();

                MinerProcess.Start();
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
                var proc = Process.GetProcessesByName(Config.Miner.ProcessName);

                if (proc != null)
                {
                    MinerProcess = proc[0];

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

        class TimerHelper
        {
            private DispatcherTimer _timer { get; set; }

            private TimeSpan _uptime { get; set; }

            private int _uptimeSeconds { get; set; }

            private int _uptimeMinutes { get; set; }

            private int _uptimeHours { get; set; }

            private int _uptimeDays { get; set; }


            public TimerHelper()
            {
                _timer = new DispatcherTimer();
                _timer.Interval = new TimeSpan(0, 0, 1);
                _timer.Tick += (s, e) => Timer_Tick();

                _timer.Start();
            }

            private void Timer_Tick()
            {
                _uptimeSeconds++;

                if (_uptimeSeconds >= 60)
                {
                    AddUptime();
                }
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
        }
    }
}
