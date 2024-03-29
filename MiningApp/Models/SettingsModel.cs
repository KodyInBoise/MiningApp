﻿using System;
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

        public GeneralSettings General { get; set; }

        public MiningSettings Mining { get; set; }


        public SettingsModel()
        {
            App = new AppSettings();
            General = new GeneralSettings();
            Mining = new MiningSettings();
        }

        public class AppSettings
        {
            public string Name { get; set; } = "MiningApp";
            public ServerHelper.VersionHelper.VersionModel AppVersion { get; set; }
        }

        public class GeneralSettings
        {
            public bool LaunchOnStartup { get; set; } = false;
            public int LaunchConfigID { get; set; } = -1;
            public bool CheckForUpdates { get; set; } = false;
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
    }    
}
