using MiningApp.LoggingUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public delegate void ExceptionEventDelegate(ExceptionArgs args);

    public enum ExceptionType
    {
        General = 0,
        Session = 1,
        Blacklist = 2,
    }

    public enum ExceptionUrgency
    {
        None = 0,
        Low = 1, 
        Medium = 2,
        High = 3
    }

    public class ExceptionArgs
    {
        public DateTime TimeStamp { get; private set; }

        public ExceptionType Type { get; set; }

        public ExceptionUrgency Urgency { get; set; }

        public bool ShowError { get; set; }

        public string Message { get; set; }

        public string LocalPath { get; set; }

        public ExceptionArgs()
        {
            TimeStamp = DateTime.Now;
        }
    }

    public class ExceptionUtil
    {
        public static ExceptionUtil Instance { get; set; }

        public static ExceptionEventDelegate Delegate { get; set; }


        public ExceptionUtil()
        {
            Instance = this;
        }

        public static void Handle(Exception ex, ExceptionType type = ExceptionType.General, ExceptionUrgency urgency = ExceptionUrgency.None, 
            string message = "", bool showError = false, string path = "")
        {
            var args = new ExceptionArgs()
            {
                Type = type,
                Urgency = ExceptionUrgency.None,
                ShowError = false,
                Message = !String.IsNullOrEmpty(message) ? message : ex.Message,
                LocalPath = path,
            };

            Delegate?.Invoke(args);

            LogHelper.AddEntry(ex);
        }
    }
}
