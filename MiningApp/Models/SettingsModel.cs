using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

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
            public List<BlacklistedItem> BlacklistedItems { get; set; } = new List<BlacklistedItem>();
        }
    }    
}
