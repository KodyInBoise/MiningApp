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
            //return Timestamp.Subtract(Started);
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
        public object Owner { get; set; }

        public DateTime Started { get; private set; }

        public DateTime LastPause { get; private set; }

        public TimerTickDelegate Delegate { get; set; }

        public Dictionary<DateTime, TimerAction> HistoryDictionary { get; private set; }

        public DateTime LastTick { get; private set; }

        public TimeSpan Interval { get; private set; }

        public DispatcherTimer GetTimer => _timer;

        
        Action _eventAction { get; set; }

        DispatcherTimer _timer { get; set; }

        bool _pauseBetweenTicks { get; set; }


        public TimerModel(object owner, TimeSpan interval, Action action = null, bool start = true, bool pauseBetweenTicks = false)
        {
            Owner = owner;
            _eventAction = action;

            Interval = interval;
            _pauseBetweenTicks = pauseBetweenTicks;

            Initialize(start);
        }

        public TimerModel(object owner, Action action = null, int interval = -1, bool start = true, bool pauseBetweenTicks = false)
        {
            var parsedInterval = interval > 0 ? new TimeSpan(0, 0, interval) : new TimeSpan(0, 0, 1);

            new TimerModel(owner, parsedInterval, action, start, pauseBetweenTicks);
        }

        void Initialize(bool start)
        {
            HistoryDictionary = new Dictionary<DateTime, TimerAction>();

            _timer = new DispatcherTimer();
            _timer.Interval = Interval;
            _timer.Tick += (s, e) => Timer_Ticked();

            if (start)
            {
                ToggleStatus(TimerAction.Create);
            }
        }

        int timerTickErrors = 0;
        async void Timer_Ticked()
        {
            try
            {
                LastTick = DateTime.Now;
                Delegate?.Invoke(new TimerTickedArgs
                {
                    Timestamp = LastTick,
                    Started = Started,
                });

                if (_eventAction == null)
                {
                    return;
                }

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
            }
            catch (Exception ex)
            {
                var message = $"An error occured with the Timer for \"{nameof(Owner)}\".";

                if (timerTickErrors >= 5)
                {
                    ToggleStatus(TimerAction.Stop);
                    message += $" Timer has had {timerTickErrors} Errors as has been stopped!";
                }
                else
                {
                    timerTickErrors++;
                }

                ExceptionUtil.Handle(ex, message: message);
            }
        }

        public void ToggleStatus(TimerAction action)
        {
            var timestamp = DateTime.Now;

            switch (action)
            {
                case TimerAction.Create:
                    Started = timestamp;
                    _timer.Start();
                    break;
                case TimerAction.Start:
                    _timer.Start();
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

            HistoryDictionary?.Add(timestamp, action);
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
