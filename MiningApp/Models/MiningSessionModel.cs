using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class MiningSessionModel
    {
        public ConfigModel Config { get; set; }

        public Process Process { get; set; }

        public MiningSessionModel(ConfigModel config)
        {
            Config = config;
        }

        public void Start()
        {
            Process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = Config.Miner.Path,
                }
            };

            Process.Start();
        }
    }
}
