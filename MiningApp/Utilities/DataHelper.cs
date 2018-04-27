using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace MiningApp
{
    public class DataHelper
    {
        public static string DataFilePath => Path.Combine(RootPath(), "simplemining.data");
        public static string MinerDirectory => Path.Combine(RootPath(), "Miners");

        private LiteDatabase _database => GetDatabase();
        private LiteCollection<MinerModel> _minerCollection => GetMinerCollection();

        public DataHelper()
        {
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

        private LiteCollection<MinerModel> GetMinerCollection()
        {
            using (_database)
            {
                return _database.GetCollection<MinerModel>("miners");
            }
        }

        public void InsertMiner(MinerModel miner)
        {
            using (_database)
            {
                _minerCollection.Insert(miner);
            }
        }

        public async Task<List<MinerModel>> GetMiners()
        {
            using (_database)
            {
                return _minerCollection.FindAll().ToList();
            }
        }

        public void UpdateMiner(MinerModel miner)
        {
            using (_database)
            {
                _minerCollection.Update(miner);
            }
        }

        public async Task<MinerModel> GetMinerByID(int ID)
        {
            using (_database) return _minerCollection.FindById(ID);
        }
    }
}
