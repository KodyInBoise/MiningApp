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
        List<MinerModel> _allMiners { get; set; } = new List<MinerModel>();

        string _minersJsonPath { get; set; } = Path.Combine(DataHelper.RootPath(), "Miners", "allminers.json");

        public ServerHelper()
        {
            _allMiners = GetAllMinersLocal();
        }

        public void AddMiner(MinerModel miner)
        {
            _allMiners.Add(miner);

            SaveMinersList();
        }

        private void SaveMinersList()
        {
            var content = JsonConvert.SerializeObject(_allMiners);

            File.WriteAllText(_minersJsonPath, content);
        }

        private List<MinerModel> GetAllMinersLocal()
        {
            try
            {
                return JsonConvert.DeserializeObject<List<MinerModel>>(_minersJsonPath);
            }
            catch
            {
                return new List<MinerModel>();
            }
        }
    }
}
