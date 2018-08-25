using IWshRuntimeLibrary;
using MiningApp.LoggingUtil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static MiningApp.SettingsModel;

namespace MiningApp
{
    public class Bootstrapper
    {
        public const char ARGUMENT_IDENTIFIER = '-';


        public static Bootstrapper Instance { get; set; }

        public static SettingsModel Settings { get; set; }

        public static string SettingsFilePath => Path.Combine(RootPath(), $"simplemining.settings");

        public static string AppTempPath => Path.Combine(RootPath(), "Temp");

        public Bootstrapper()
        {
            Instance = this;

            Settings = GetLocalSettings();
        }

        public static async void Startup()
        {
            Instance = new Bootstrapper();

            await KillExistingProcesses();
        }
        
        static async Task KillExistingProcesses()
        {
            /*
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
            */
        }

        public static string RootPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SimpleMining");
        }

        public SettingsModel GetLocalSettings()
        {
            try
            {
                if (System.IO.File.Exists(SettingsFilePath))
                {
                    var content = System.IO.File.ReadAllText(SettingsFilePath);

                    if (!String.IsNullOrEmpty(content))
                    {
                        var settings = JsonConvert.DeserializeObject<SettingsModel>(content);

                        return settings ?? new SettingsModel();
                    }
                }

                return new SettingsModel();
            }
            catch (Exception ex)
            {
                LogHelper.AddEntry(ex);

                return new SettingsModel();
            }
        }

        public void SaveLocalSettings(SettingsModel settings = null)
        {
            try
            {
                settings = settings != null ? settings : Settings;

                var content = JsonConvert.SerializeObject(settings);

                System.IO.File.WriteAllText(SettingsFilePath, content);

                EnforceSettings();
            }
            catch (Exception ex)
            {
                LogHelper.AddEntry(ex);
            }
        }

        void EnforceSettings()
        {
            try
            {
                var startupShortcutPath = Path.Combine(GetStartupPath(), "SimpleMining.lnk");

                if (Settings.General.LaunchOnStartup)
                {
                    CreateShortcut(startupShortcutPath);
                }
                else
                {
                    if (System.IO.File.Exists(startupShortcutPath))
                    {
                        System.IO.File.Delete(startupShortcutPath);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddEntry(ex);
            }
        }

        string GetStartupPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        }

        void CreateShortcut(string path)
        {
            try
            {
                var startupArg = Settings.General.LaunchOnStartup ? "-startup" : string.Empty;

                var shell = new WshShell();
                var windowsApplicationShortcut = (IWshShortcut)shell.CreateShortcut(path);
                windowsApplicationShortcut.Description = App.Current.ToString();
                windowsApplicationShortcut.WorkingDirectory = Directory.GetCurrentDirectory();
                windowsApplicationShortcut.TargetPath = Path.Combine(Directory.GetCurrentDirectory(), $"MiningApp.exe");
                windowsApplicationShortcut.Arguments = $"{startupArg}";
                windowsApplicationShortcut.Save();
            }
            catch (Exception ex)
            {
                LogHelper.AddEntry(ex);
            }
        }

        public static async Task<List<string>> GetParseArguments()
        {
            var procArg = Process.GetCurrentProcess().MainModule.FileName;
            var rawArgs = Environment.GetCommandLineArgs().ToList();

            var parsedArgs = new List<string>();
            foreach (var arg in rawArgs)
            {
                if (arg != procArg)
                {
                    parsedArgs.Add(arg.TrimStart(ARGUMENT_IDENTIFIER).ToLower());
                }
            }
           
            return parsedArgs;
        }
    }
}
