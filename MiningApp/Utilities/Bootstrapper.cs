using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MiningApp.SettingsModel;

namespace MiningApp
{

    public class Bootstrapper
    {
        public static Bootstrapper Instance { get; set; }

        public static SettingsModel Settings { get; set; }

        public static string SettingsFilePath => Path.Combine(RootPath(), $"{AppSettings.AppName}.settings");

        public Bootstrapper()
        {
            Instance = this;

            Settings = new SettingsModel();
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

        public static string RootPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SimpleMining");
        }
    }
}
