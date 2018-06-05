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
        public GeneralSettings General { get; set; }


        public SettingsModel()
        {
            General = new GeneralSettings();
        }

        public class AppSettings
        {
            public string Name { get; set; } = "MiningApp";

            public double Version { get; set; } = 0.0;

            public DateTime LastUpdate { get; set; }
        }

        public class GeneralSettings
        {
            public bool LaunchOnStartup { get; set; } = false;

            public bool CheckForUpdates { get; set; } = false;
        }
    }
}
