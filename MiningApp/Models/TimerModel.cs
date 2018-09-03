using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MiningApp
{
    public delegate void TimerTickDelegate(TimerTickedArgs args);

    public class TimerTickedArgs
    {
        public DateTime Timestamp { get; set; }
        public DateTime Started { get; set; }
        public TimeSpan Uptime => GetUptime();

        TimeSpan GetUptime()
        {
            return Timestamp.Subtract(Started);
        }
    }

    public enum TimerAction
    {
        Stop = 0,
        Start = 1, 
        Pause = 2, 
        Reset = 3,
        Create = 4,
    }


    public class TimerModel
    {
        public DateTime Started { get; private set; }

        public DateTime LastPause { get; private set; }

        public TimerTickDelegate Delegate { get; private set; }

        public Dictionary<DateTime, TimerAction> HistoryDictionary { get; private set; }

        public DateTime LastTick { get; private set; }

        public int Interval { get; private set; }

        
        Action _eventAction { get; set; }

        DispatcherTimer _timer { get; set; }

        bool _pauseBetweenTicks { get; set; }


        public TimerModel(Action action, int interval = -1, bool start = false, bool pauseBetweenTicks = false)
        {
            Interval = interval > 0 ? interval : 1;
            _eventAction = action;
            _pauseBetweenTicks = pauseBetweenTicks;

            Initialize(start);
        }

        void Initialize(bool start)
        {
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, Interval);
            _timer.Tick += (s, e) => Timer_Ticked();

            HistoryDictionary = new Dictionary<DateTime, TimerAction>();
            HistoryDictionary.Add(DateTime.Now, TimerAction.Create);

            if (start)
            {
                ToggleStatus(TimerAction.Start);
            }
        }

        async void Timer_Ticked()
        {
            LastTick = DateTime.Now;

            if (_pauseBetweenTicks)
            {
                _timer.Stop();
                await Task.Run(_eventAction);
                _timer.Start();
            }
            else
            {
                Task.Run(_eventAction);
            }

            Delegate?.Invoke(new TimerTickedArgs
            {
                Timestamp = LastTick,
                Started = Started,
            });
        }

        void ToggleStatus(TimerAction action)
        {
            var timestamp = DateTime.Now;

            switch (action)
            {
                case TimerAction.Start:
                    _timer.Start();
                    Started = Started == null ? timestamp : Started;
                    break;
                case TimerAction.Pause:
                    _timer.Stop();
                    LastPause = timestamp;
                    break;
                case TimerAction.Reset:
                    _timer.Stop();
                    Initialize(true);
                    break;
                default:
                    break;
            }

            HistoryDictionary.Add(timestamp, action);
        }
        
        public void Dispose()
        {
            ToggleStatus(TimerAction.Stop);
            _timer.Tick -= (s, e) => _eventAction();
            _timer = null;
            Delegate = null;
        }
    }
}
