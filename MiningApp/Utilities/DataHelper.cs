﻿using System;
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
        public static string DataFilePath => Path.Combine(RootPath(), "simplemining.data");

        public static string MinerDirectory => Path.Combine(RootPath(), "Miners");

        public static string UserSettingsPath => Path.Combine(RootPath(), "usersettings.json");

        private LiteDatabase _database => GetDatabase();

        private LiteCollection<MinerConfigModel> _minerConfigCollection => GetMinerCollection();

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

        private LiteCollection<MinerConfigModel> GetMinerCollection()
        {
            using (_database)
            {
                return _database.GetCollection<MinerConfigModel>("minerconfigs");
            }
        }

        public void InsertMiner(MinerConfigModel miner)
        {
            using (_database)
            {
                _minerConfigCollection.Insert(miner);
            }
        }

        public async Task<List<MinerConfigModel>> GetMiners()
        {
            using (_database)
            {
                return _minerConfigCollection.FindAll().ToList();
            }
        }

        public void UpdateMiner(MinerConfigModel miner)
        {
            using (_database)
            {
                _minerConfigCollection.Update(miner);
            }
        }

        public async Task<MinerConfigModel> GetMinerByID(int minerID)
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
    }
}
