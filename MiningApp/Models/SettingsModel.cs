using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Newtonsoft.Json;

namespace MiningApp
{
    public class SettingsModel
    {
        public AppSettings App { get; set; }

        public UserSettings User { get; set; }

        public GeneralSettings General { get; set; }

        public MiningSettings Mining { get; set; }

        public ServerSettings Server { get; set; }


        public SettingsModel()
        {
            App = new AppSettings();
            User = new UserSettings();
            General = new GeneralSettings();
            Mining = new MiningSettings();
            Server = new ServerSettings();
        }

        public class AppSettings
        {
            public string Name { get; set; } = "MiningApp";
            public ServerHelper.VersionHelper.VersionModel AppVersion { get; set; }
        }

        public class GeneralSettings
        {
            public bool UseServer { get; set; } = true;
            public bool LaunchOnStartup { get; set; } = false;
            public int LaunchConfigID { get; set; } = -1;
            public bool CheckForUpdates { get; set; } = false;
        }

        public class UserSettings
        {
            public string UserID { get; set; }
            public string EmailAddress { get; set; }
            public bool RequiresLogin { get; set; } = true;
        }

        public class MiningSettings
        {
            public bool UseBlackList { get; set; } = true;
            public int BlacklistCheckInterval { get; set; } = 10;
            public List<BlacklistItem> BlacklistedItems { get; set; } = new List<BlacklistItem>();

            [JsonIgnore]
            public List<string> ExcludeFromBlacklistPaths { get; set; } = new List<string>();

            public async Task<List<BlacklistItem>> GetAllBlacklistedProcesses()
            {
                var procs = new List<BlacklistItem>();
                var items = BlacklistedItems;

                foreach (var item in items)
                {
                    if (item.BlacklistType == BlacklistedItemType.Executable && !ExcludeFromBlacklistPaths.Contains(item.FullPath))
                    {
                        procs.Add(item);
                    }
                    else if (item.BlacklistType == BlacklistedItemType.Directory && !ExcludeFromBlacklistPaths.Contains(item.FullPath))
                    {
                        var paths = await Task.Run(item.GetDirectoryExecutablePaths);
                        paths.ForEach(x => procs.Add(new BlacklistItem(BlacklistedItemType.Executable, x)));
                    }
                }

                return procs;
            }
        }

        public class ServerSettings
        {
            public string LocalClientID { get; set; }
            public bool UserAuthenticated { get; set; } = false;
            public DateTime LastCheckin { get; set; }
            public string UserID => Bootstrapper.User.ID;
        }
    }    
}
