using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Newtonsoft.Json;
using MiningApp.LoggingUtil;

namespace MiningApp
{
    public class DataHelper
    {
        public static DataHelper Instance { get; set; }

        public static string DataFilePath => Path.Combine(RootPath(), "simplemining.data");

        public static string MinerDirectory => Path.Combine(RootPath(), "Miners");

        public static string UserSettingsPath => Path.Combine(RootPath(), "usersettings.json");


        private static LiteDatabase localDB { get; set; }


        private LiteDatabase _database => GetDatabase();

        private LiteCollection<MiningRuleModel> _miningRuleConfigCollection => GetMiningRuleCollection();

        private LiteCollection<WalletConfigModel> _walletConfigCollection => GetWalletCollection();

        private LiteCollection<PoolConfigModel> _poolConfigCollection => GetPoolCollection();

        private LiteCollection<MinerConfigModel> _minerConfigCollection => GetMinerConfigCollection();

        private LiteCollection<LogEntry> _generalLogCollection => GetGeneralLogCollection();

        private LiteCollection<LogEntry> _errorLogCollection => GetErrorLogCollection();

        public DataHelper()
        {
            Instance = this;

            var root = RootPath();
             
            if (!Directory.Exists(root)) Directory.CreateDirectory(root);
            if (!Directory.Exists(MinerDirectory)) Directory.CreateDirectory(MinerDirectory);

            localDB = GetDatabase();
        }

        public static string RootPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SimpleMining");
        }

        private LiteDatabase GetDatabase()
        {
            return new LiteDatabase(DataFilePath);
        }

        private LiteCollection<MiningRuleModel> GetMiningRuleCollection()
        {
            using (_database)
            {
                return _database.GetCollection<MiningRuleModel>("miningruleconfigs");
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

        private LiteCollection<MinerConfigModel> GetMinerConfigCollection()
        {
            using (_database)
            {
                return _database.GetCollection<MinerConfigModel>("minerconfigs");
            }
        }

        private LiteCollection<LogEntry> GetGeneralLogCollection()
        {
            using (_database)
            {
                return _database.GetCollection<LogEntry>("logs-general");
            }
        }

        private LiteCollection<LogEntry> GetErrorLogCollection()
        {
            using (_database)
            {
                return _database.GetCollection<LogEntry>("logs-error");
            }
        }

        public void InsertMiningRule(MiningRuleModel miner)
        {
            using (_database)
            {
                _miningRuleConfigCollection.Insert(miner);
            }
        }

        public async Task<List<MiningRuleModel>> GetAllMiningRules()
        {
            using (_database)
            {
                return _miningRuleConfigCollection.FindAll().ToList();
            }
        }

        public void UpdateMiningRule(MiningRuleModel miner)
        {
            using (_database)
            {
                _miningRuleConfigCollection.Update(miner);
            }
        }

        public async Task<MiningRuleModel> GetMiningRuleByID(int minerID)
        {
            using (_database)
            {
                return _miningRuleConfigCollection.FindById(minerID);
            }
        }

        public void DeleteMiningRule(int minerID)
        {
            using (_database)
            {
                _miningRuleConfigCollection.Delete(minerID);
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

        public void SaveWallet(WalletConfigModel wallet)
        {
            using (_database)
            {
                if (wallet.ID > 0)
                {
                    _walletConfigCollection.Update(wallet);
                }
                else
                {
                    _walletConfigCollection.Insert(wallet);
                }
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

        public void SavePoolConfig(PoolConfigModel pool)
        {
            using (_database)
            {
                if (pool.ID > 0)
                {
                    _poolConfigCollection.Update(pool);
                }
                else
                {
                    _poolConfigCollection.Insert(pool);
                }
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

        public List<PoolConfigModel> GetAllPoolConfigs()
        {
            using (_database)
            {
                return _poolConfigCollection.FindAll().ToList();
            }
        }

        public void SaveMiner(MinerConfigModel miner)
        {
            using (_database)
            {
                if (miner.ID > 0)
                {
                    _minerConfigCollection.Update(miner);
                }
                else
                {
                    _minerConfigCollection.Insert(miner);
                }
            }
        }

        public void UpdateMinerConfig(MinerConfigModel miner)
        {
            using (_database)
            {
                _minerConfigCollection.Update(miner);
            }
        }

        public void DeleteMinerConfig(MinerConfigModel miner)
        {
            using (_database)
            {
                _minerConfigCollection.Delete(miner.ID);
            }
        }

        public async Task<List<MinerConfigModel>> GetAllMinerConfigs()
        {
            using (_database)
            {
                return _minerConfigCollection.FindAll().ToList();
            }
        }

        public void InsertLogEntry(LogEntry entry)
        {
            using (_database)
            {
                switch (entry.Type)
                {
                    case LogType.General:
                        _generalLogCollection.Insert(entry);
                        break;
                    case LogType.Error:
                        _errorLogCollection.Insert(entry);
                        break;
                    default:
                        break;
                }
            }
        }

        public void DeleteLogEntry(LogEntry entry)
        {
            using (_database)
            {
                switch (entry.Type)
                {
                    case LogType.General:
                        _generalLogCollection.Update(entry);
                        break;
                    case LogType.Error:
                        _errorLogCollection.Update(entry);
                        break;
                    default:
                        break;
                }
            }
        }

        public List<LogEntry> GetLogEntries(LogType type)
        {
            using (_database)
            {
                switch (type)
                {
                    case LogType.General:
                        return _generalLogCollection.FindAll().ToList();
                    case LogType.Error:
                        return _errorLogCollection.FindAll().ToList();
                    default:
                        return new List<LogEntry>();
                }
            }
        }
    }
}
