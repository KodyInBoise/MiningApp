using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp.LoggingUtil
{
    public enum LogType : int
    {
        General = 0, //App startup, shutdown, config / setting changes, etc
        Error = 1,
        Session = 2, //Start, stop, restarts, etc
        Server = 3,
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

        public static List<LogEntry> GeneralLogEntries => AppData.GetLogEntries(LogType.General);

        public static List<LogEntry> ErrorLogEntries => AppData.GetLogEntries(LogType.Error);

        public static List<LogEntry> SessionLogEntries => AppData.GetLogEntries(LogType.Session);

        public static List<LogEntry> ServerLogEntries => AppData.GetLogEntries(LogType.Server);


        private ViewLogsWindow _logWindow { get; set; }


        public LogHelper()
        {
            Instance = this;

            AppData = DataHelper.Instance ?? new DataHelper();
        }

        public static void AddEntry(string message)
        {
            Console.WriteLine(message);

            AddEntry(LogType.General, message);
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

                AppData?.InsertLogEntry(entry);
            }
            catch (LogException ex)
            {
                ex.Handle();
            }
        }

        public static void AddEntry(Exception ex, string message = "")
        {
            message = !String.IsNullOrEmpty(message) ? $"{message} - {ex.Message}" : ex.Message;
            AddEntry(LogType.Error, ex.Message);
        }

        public void ShowWindow()
        {
            _logWindow?.Close();

            _logWindow = new ViewLogsWindow();
        }

        public static List<string> LogCategories()
        {
            return new List<string>()
            {
                "General",
                "Error",
                "Session",
                "Server",
            };
        }

        public static async Task ClearEntries(LogType type)
        {
            AppData.ClearCategoryLogs(type);
        }
    }
}
