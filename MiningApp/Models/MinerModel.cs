using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class MinerModel
    {
        public string Name { get; set; }

        public DateTime AddedTimestamp { get; set; }

        public string LocalDirectory  => GetLocalDirectory();

        public List<string> SupportedCoins()
        {
            return new List<string>()
            {
                "PIRL",
                "VTC",
            };
        }

        private string GetLocalDirectory()
        {
            return Path.Combine(DataHelper.MinerDirectory, Name);
        }
    }
}
