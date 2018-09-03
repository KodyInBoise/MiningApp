using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
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
        public DateTime TimeStamp { get; set; }

        public ExceptionType Type { get; set; }

        public ExceptionUrgency Urgency { get; set; } = ExceptionUrgency.Low;

        public string LocalPath { get; set; }

        public ExceptionArgs()
        {
            TimeStamp = DateTime.Now;
        }
    }

    public class ExceptionManagementUtil
    {
        public static ExceptionManagementUtil Instance { get; set; }


        public ExceptionManagementUtil()
        {
            Instance = this;
        }
    }
}
