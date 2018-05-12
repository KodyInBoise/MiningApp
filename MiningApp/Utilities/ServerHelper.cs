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

        public void AddMiner(MinerConfigModel miner)
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
    }
}
