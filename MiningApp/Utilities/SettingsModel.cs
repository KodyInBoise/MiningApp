using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using static MiningApp.SettingsModel;

namespace MiningApp
{
    public class SettingStorage
    {
        public static LiteDatabase Database { get; set; }


        public SettingStorage()
        {
            Database = new LiteDatabase(Bootstrapper.SettingsFilePath);
        }

        public class General
        {
            public static GeneralSettings Settings => Collection.FindAll().FirstOrDefault();


            static LiteCollection<GeneralSettings> Collection => GetCollection();


            static LiteCollection<GeneralSettings> GetCollection()
            {
                using (Database)
                {
                    return Database.GetCollection<GeneralSettings>("General");
                }
            }

            public static void Save(GeneralSettings settings)
            {
                using (Database)
                {
                    Collection.Upsert(settings);
                }
            }
        }

    }


    public class SettingsModel
    {
        public static SettingsModel Instance { get; set; }

        public static SettingStorage Storage { get; set; }


        public SettingsModel()
        {
            Instance = this;

            Storage = new SettingStorage();
        }

        public class AppSettings
        {
            public static string AppName = "MiningApp";

            public static int CurrentProcessID => Process.GetCurrentProcess().Id;


            public AppSettings()
            {

            }
        }

        public class GeneralSettings
        {
            public int ID { get; set; }

            public bool LaunchOnStartup { get; set; } = false;
        }
    }
}
