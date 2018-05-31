﻿using MiningApp.UI;
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

        public DateTime LastOutputTimestamp { get; set; }

        public TimeSpan Uptime => GetUptime();

        public string UptimeString => _sessionTimer.GetUptimeFriendlyString();

        public string NewOutput => GetNewOutput();

        public string AllOutput => _output;

        
        TimerHelper _sessionTimer { get; set; }

        string _output { get; set; } = "";

        bool _redirectOutput { get; set; } = true;

        public MiningSessionModel(ConfigModel config)
        {
            StartTime = DateTime.Now;
            Config = config;

            _sessionTimer = new TimerHelper();
        }

        public async Task Start()
        {
            if (!IsRunning())
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

                SetMinerSettings();

                MinerProcess.Start();

                while (!MinerProcess.StandardOutput.EndOfStream)
                {
                    var output = MinerProcess.StandardOutput.ReadLine();

                    AppendOutput(output);
                }
            }
        }

        private void AppendOutput(string output)
        {
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
            catch (System.IndexOutOfRangeException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }

        public void Close()
        {
            WindowController.MiningSessions.Remove(this);

            MinerProcess.Close();
        }

        public DispatcherTimer GetTimer()
        {
            return _sessionTimer.GetTimer();
        }

        bool OutputIsStale()
        {
            return DateTime.Now > LastOutputTimestamp.AddMinutes(Config.StaleOutputThreshold);
        }

        public async Task CheckForStaleMiners()
        {
            if (OutputIsStale())
            {
                RestartMiner();
            }
        }

        public void RestartMiner()
        {

        }

        class TimerHelper
        {
            private DispatcherTimer _timer { get; set; }

            private TimeSpan _uptime { get; set; }

            private int _uptimeSeconds { get; set; }

            private int _uptimeMinutes { get; set; }

            private int _uptimeHours { get; set; }

            private int _uptimeDays { get; set; }

            private int _staleOutputSeconds { get; set; }

            public TimerHelper()
            {
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
        }
    }
}