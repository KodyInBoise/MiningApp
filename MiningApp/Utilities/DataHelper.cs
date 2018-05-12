using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Newtonsoft.Json;

namespace MiningApp
{
    public class DataHelper
    {
        public static DataHelper Instance { get; set; }



        public static string DataFilePath => Path.Combine(RootPath(), "simplemining.data");

        public static string MinerDirectory => Path.Combine(RootPath(), "Miners");

        public static string UserSettingsPath => Path.Combine(RootPath(), "usersettings.json");



        private LiteDatabase _database => GetDatabase();

        private LiteCollection<MiningRuleModel> _minerConfigCollection => GetMinerCollection();

        private LiteCollection<WalletConfigModel> _walletConfigCollection => GetWalletCollection();

        private LiteCollection<PoolConfigModel> _poolConfigCollection => GetPoolCollection();

        public DataHelper()
        {
            Instance = this;

            var root = RootPath();
             
            if (!Directory.Exists(root)) Directory.CreateDirectory(root);
            if (!Directory.Exists(MinerDirectory)) Directory.CreateDirectory(MinerDirectory);
        }

        public static string RootPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SimpleMining");
        }

        private LiteDatabase GetDatabase()
        {
            return new LiteDatabase(DataFilePath);
        }

        private LiteCollection<MiningRuleModel> GetMinerCollection()
        {
            using (_database)
            {
                return _database.GetCollection<MiningRuleModel>("minerconfigs");
            }
        }

        private LiteCollection<WalletConfigModel> GetWalletCollection()
        {
            using (_database)
            {
                return _database.GetCollection<WalletConfigModel>("walletconfigs");
            }
        }

        private LiteCollection<PoolConfigModel> GetPoolCollection()
        {
            using (_database)
            {
                return _database.GetCollection<PoolConfigModel>("poolconfigs");
            }
        }

        public void InsertMiner(MiningRuleModel miner)
        {
            using (_database)
            {
                _minerConfigCollection.Insert(miner);
            }
        }

        public async Task<List<MiningRuleModel>> GetAllMiners()
        {
            using (_database)
            {
                return _minerConfigCollection.FindAll().ToList();
            }
        }

        public void UpdateMiner(MiningRuleModel miner)
        {
            using (_database)
            {
                _minerConfigCollection.Update(miner);
            }
        }

        public async Task<MiningRuleModel> GetMinerByID(int minerID)
        {
            using (_database)
            {
                return _minerConfigCollection.FindById(minerID);
            }
        }

        public void DeleteMiner(int minerID)
        {
            using (_database)
            {
                _minerConfigCollection.Delete(minerID);
            }
        }

        public static void SaveUserSettings(UserModel user)
        {
            var content = JsonConvert.SerializeObject(user);

            File.WriteAllText(UserSettingsPath, content);
        }

        public static UserModel LoadUserSettings()
        {
            try
            {
                var content = File.ReadAllText(UserSettingsPath);

                return JsonConvert.DeserializeObject<UserModel>(content);
            }
            catch
            {
                return new UserModel();
            }
        }

        public void InsertWalletConfig(WalletConfigModel wallet)
        {
            using (_database)
            {
                _walletConfigCollection.Insert(wallet);
            }
        }

        public void UpdateWalletConfig(WalletConfigModel wallet)
        {
            using (_database)
            {
                _walletConfigCollection.Update(wallet);
            }
        }

        public void DeleteWalletConfig(WalletConfigModel wallet)
        {
            using (_database)
            {
                _walletConfigCollection.Delete(wallet.ID);
            }
        }

        public List<WalletConfigModel> GetWalletConfigs()
        {
            using (_database)
            {
                return _walletConfigCollection.FindAll().ToList();
            }
        }

        public void InsertPoolConfig(PoolConfigModel pool)
        {
            using (_database)
            {
                _poolConfigCollection.Insert(pool);
            }
        }

        public void UpdatePoolConfig(PoolConfigModel pool)
        {
            using (_database)
            {
                _poolConfigCollection.Update(pool);
            }
        }

        public void DeletePoolConfig(PoolConfigModel pool)
        {
            using (_database)
            {
                _poolConfigCollection.Delete(pool.ID);
            }
        }

        public List<PoolConfigModel> GetAllPools()
        {
            using (_database)
            {
                return _poolConfigCollection.FindAll().ToList();
            }
        }
    }
}
