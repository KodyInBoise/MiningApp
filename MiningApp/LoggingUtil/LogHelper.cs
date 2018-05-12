using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp.LoggingUtil
{
    public enum LogType
    {
        General,
        Error,
    }

    public class LogException : Exception
    {
        public void Handle()
        {
            throw this;
        }
    }

    public class LogEntry
    {
        public int ID { get; set; }

        public DateTime Timestamp { get; set; }

        public LogType Type { get; set; }

        public string Message { get; set; }

        public LogEntry()
        {
            Timestamp = DateTime.Now;
        }
    }

    public class LogHelper
    {
        public static LogHelper Instance { get; set; }

        public static DataHelper AppData { get; set; }

        public List<LogEntry> GeneralLogEntries => AppData.GetLogEntries(LogType.General);

        public List<LogEntry> ErrorLogEntries => AppData.GetLogEntries(LogType.Error);


        private ViewLogsWindow _logWindow { get; set; }


        public LogHelper()
        {
            Instance = this;

            AppData = DataHelper.Instance ?? new DataHelper();
        }

        public static void AddEntry(LogType type, string message)
        {
            try
            {
                var entry = new LogEntry
                {
                    Type = type,
                    Message = message
                };

                AppData.InsertLogEntry(entry);
            }
            catch (LogException ex)
            {
                ex.Handle();
            }
        }

        public static void AddEntry(Exception ex)
        {
            AddEntry(LogType.Error, ex.Message);
        }

        public void ShowWindow()
        {
            _logWindow?.Close();

            _logWindow = new ViewLogsWindow();
        }
    }
}
