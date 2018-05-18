using LiteDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiningApp.UI;

namespace MiningApp
{
    public class MinerConfigModel
    {
        public int ID { get; set; }

        public string ServerID { get; set; }

        public DateTime CreatedTimestamp { get; set; }

        public string Name { get; set; }

        public List<string> Cryptos { get; set; } = new List<string>();

        public List<string> Tags { get; set; } = new List<string>();

        public List<PoolConfigModel> Pools { get; set; } = new List<PoolConfigModel>();

        public string Path { get; set; }

        public MinerStatus Status { get; set; }

        public MinerType Type { get; set; }


        [BsonIgnore]
        public string LocalDirectory => GetLocalDirectory();

        [BsonIgnore]
        public string TrimmedPath => ElementHelper.TrimPath(Path, 30);

        [BsonIgnore]
        public string CryptosString => GetCryptosString();


        public MinerConfigModel()
        {

        }

        public override string ToString()
        {
            return Name;
        }

        private string GetLocalDirectory()
        {
            return System.IO.Path.Combine(DataHelper.MinerDirectory, Name);
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

        private string GetCryptosString()
        {
            var cryptosString = "";

            Cryptos.ForEach(x => cryptosString += $"{CryptoHelper.Instance.GetCryptoSymbolFromName(x)}, ");
            cryptosString.TrimEnd(' ', ',');

            return cryptosString;
        }
    }
}
