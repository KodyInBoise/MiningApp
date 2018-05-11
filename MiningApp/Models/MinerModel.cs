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
        public DateTime AddedTimestamp { get; set; }

        public string Name { get; set; }

        public List<string> Cryptos { get; set; }

        public List<string> Tags { get; set; }

        public FileInfo File { get; set; }

        public string LocalDirectory  => GetLocalDirectory();

        public MinerModel()
        {
            Cryptos = new List<string>();
            Tags = new List<string>();
        }

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

        public async Task<bool> AddCoin(string name)
        {
            try
            {
                var crypto = await CryptoHelper.Instance.CreateCryptoFromName(name);

                Cryptos.Add(crypto.Name);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
