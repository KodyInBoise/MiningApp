using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class MinerConfigModel
    {
        public DateTime CreatedTimestamp { get; set; }

        public string Name { get; set; }

        public List<string> Cryptos { get; set; } = new List<string>();

        public List<string> Tags { get; set; } = new List<string>();

        public FileInfo File { get; set; }

        [JsonIgnore]
        public string LocalDirectory  => GetLocalDirectory();



        public MinerConfigModel()
        {

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
