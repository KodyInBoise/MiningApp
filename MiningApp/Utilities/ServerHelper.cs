using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MiningApp
{
    public class ServerHelper
    {
        public static ServerHelper Instance { get; set; }



        List<MinerConfigModel> _allMiners { get; set; } = new List<MinerConfigModel>();

        string _minersJsonPath { get; set; } = Path.Combine(DataHelper.RootPath(), "Miners", "allminers.json");



        public ServerHelper()
        {
            Instance = this;

            _allMiners = GetMiners();
        }

        public void UploadMiner(MinerConfigModel miner)
        {
            _allMiners.Add(miner);

            SaveMinersList();
        }

        private void SaveMinersList()
        {
            var content = JsonConvert.SerializeObject(_allMiners);

            File.WriteAllText(_minersJsonPath, content);
        }

        public List<MinerConfigModel> GetMiners()
        {
            try
            {
                return JsonConvert.DeserializeObject<List<MinerConfigModel>>(File.ReadAllText(_minersJsonPath));
            }
            catch
            {
                return new List<MinerConfigModel>();
            }
        }

        public string GetUniqueMinerID()
        {
            var newID = GenerateUniqueID();

            while (!UniqueMinerIDAvailable(newID))
            {
                newID = GenerateUniqueID();
            }

            return newID;
        }

        private string GenerateUniqueID()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }

        private bool UniqueMinerIDAvailable(string newID)
        {
            var miner = _allMiners.Find(x => x.ServerID == newID);

            return miner == null ? true : false;
        }
    }
}
