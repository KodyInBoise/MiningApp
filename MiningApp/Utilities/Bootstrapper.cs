using IWshRuntimeLibrary;
using MiningApp.LoggingUtil;
using MiningApp.UI;
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
    public delegate void UserAuthenticationChangedDelegate(UserAuthenticationChangedArgs args);

    public enum UserAuthenticationStatus
    {
        Disconnected = 0,
        Connected = 1,
    }

    public class UserAuthenticationChangedArgs
    {
        public DateTime Timestamp { get; private set; }
        public UserAuthenticationStatus Status { get; private set; }

        public UserAuthenticationChangedArgs(UserAuthenticationStatus status)
        {
            Timestamp = DateTime.Now;
            Status = status;
        }
    }

    public class Bootstrapper
    {
        public const char ARGUMENT_IDENTIFIER = '-';


        public static Bootstrapper Instance { get; set; }

        public static UserModel User => Instance._localUser;

        public static UserAuthenticationChangedDelegate UserAuthenticationDelegate { get; set; }

        public static LocalClientModel Client { get; set; }

        public static ServerHelper ServerHelper { get; set; }

        public TimerModel HeartbeatTimer { get; set; }

        public static SettingsModel Settings { get; set; }

        public static string SettingsFilePath => Path.Combine(RootPath(), $"simplemining.settings");

        public static string AppTempPath => Path.Combine(RootPath(), "Temp");

        public void SetUser(UserModel user, bool authenticated = false) => SetLocalUser(user, authenticated);

        
        UserModel _localUser { get; set; }


        public Bootstrapper()
        {
            Instance = this;

            HeartbeatTimer = new TimerModel(Application.Current, Heartbeat_Tick);
            Settings = GetLocalSettings();

            ServerHelper = new ServerHelper();
            ServerHelper = new ServerHelper();
            Client = new LocalClientModel();

            SetLocalUser();

            UserAuthenticationDelegate += UserAuthenticationDelegate_Invoked;
        }

        public static async void Startup()
        {
            Instance = new Bootstrapper();
        }

        void UserAuthenticationDelegate_Invoked(UserAuthenticationChangedArgs args)
        {
            if (args.Status == UserAuthenticationStatus.Connected)
            {
                Settings.Server.UserAuthenticated = true;
                User.LastServerLogin = args.Timestamp;
            }
            else
            {
                Settings.Server.UserAuthenticated = false;
            }
        }

        void Heartbeat_Tick()
        {
            Task.Run(Blacklist_Heartbeat);
        }

        int _blacklistCounter = 0;
        bool _blacklistBusy = false;
        async Task Blacklist_Heartbeat()
        {
            if (!Settings.Mining.UseBlackList || _blacklistBusy)
            {
                return;
            }

            _blacklistBusy = true;

            if (_blacklistCounter >= Settings.Mining.BlacklistCheckInterval)
            {
                await Task.Run(ProcessWatcher.RunBlacklistCheck);

                _blacklistCounter = 0;
            }
            else
            {
                _blacklistCounter++;
            }

            _blacklistBusy = false;
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

        public static void SaveLocalSettings(SettingsModel settings = null)
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

        static void EnforceSettings()
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

        async void SetLocalUser(UserModel setUser = null, bool authenticated = false)
        {
            // NEED TO CHECK AGAINST CLIENT LIST AND IF THIS IS A NEW CLIENT, NOT AUTHENTICATE USER

            try
            {
                if (setUser != null)
                {
                    if (authenticated || Settings.Server.UserAuthenticated)
                    {
                        _localUser = setUser;

                        Settings.User.UserID = User.ID;
                        Settings.User.Email = User.Email;

                        SaveLocalSettings();
                    }
                }
                else if (!String.IsNullOrEmpty(Settings.User.Email))
                {
                    var user = await ServerHelper.GetUserByEmail(Settings.User.Email);

                    if (user.RequiresLogin)
                    {
                        Settings.Server.UserAuthenticated = false;
                    }
                    else
                    {
                        Settings.Server.UserAuthenticated = true;

                        _localUser = user;
                        Settings.User.UserID = User.ID;
                        Settings.User.Email = User.Email;
                    }
                }
                else
                {
                    Settings.Server.UserAuthenticated = false;
                }
            }
            catch (Exception ex)
            {
                _localUser = null;

                ExceptionUtil.Handle(ex);
            }
        }

        static string GetStartupPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        }

        static void CreateShortcut(string path)
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
