using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class AppSettings
    {
        public static string AppName = "MiningApp";

        public static int CurrentProcessID => Process.GetCurrentProcess().Id;


        public AppSettings()
        {

        }
    }

    public class Bootstrapper
    {
        public static Bootstrapper Instance { get; set; }

        public AppSettings Settings { get; set; }

        public Bootstrapper()
        {
            Instance = this;
            Settings = new AppSettings();
        }

        public static async void Startup()
        {
            Instance = new Bootstrapper();

            await KillExistingProcesses();
        }
        
        static async Task KillExistingProcesses()
        {
            var procs = Process.GetProcessesByName(AppSettings.AppName).ToList();

            var currentProcess = Process.GetProcessById(AppSettings.CurrentProcessID);
            procs.Remove(currentProcess);

            foreach (var proc in procs)
            {
                if (proc.Id != AppSettings.CurrentProcessID)
                {
                    proc.Kill();
                }
            }
        }
    }
}
