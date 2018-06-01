using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class MinerSettings
    {
        public class CCMiner
        {
            public static string MinerName = "CCMiner";

            public static string RootDirectory => Path.Combine(DataHelper.DataFilePath, DataHelper.MinerDirectory, MinerName);

            public static string DefaultSettingsPath => Path.Combine(RootDirectory, "defaultconf.json");

            public static string ConfigPath => Path.Combine(RootDirectory, "ccminer.conf");


            private static string _defaultPool = "stratum+tcp://vtc.coinfoundry.org:3096";

            private static string _defaultWallet = "VuV21zhJqEeHHFar9E5hAooPwT6WhGLSCB";


            public static async Task<string> SaveParams(string pool = "", string wallet = "")
            {
                try
                {
                    pool = String.IsNullOrEmpty(pool) ? _defaultPool : pool;
                    wallet = String.IsNullOrEmpty(wallet) ? _defaultWallet : wallet;

                    string defaultJson = File.ReadAllText(DefaultSettingsPath);

                    dynamic DynamicObject = JsonConvert.DeserializeObject<dynamic>(defaultJson);
                    DynamicObject.url = pool;
                    DynamicObject.user = wallet;

                    var content = JsonConvert.SerializeObject(DynamicObject);
                    File.WriteAllText(ConfigPath, content);

                    return "success";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
    }
}
