using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MiningApp.LoggingUtil;
using Newtonsoft.Json;
using static MiningApp.ServerHelper.VersionHelper;
using MySql.Data.MySqlClient;

namespace MiningApp
{ 
    public class ServerHelper
    {
        public static ServerHelper Instance { get; set; }

        public MySqlConnection GetDatabaseConnection() => _databaseHelper.GetConnection();

        #region Clients
        public string NewClientID => _databaseHelper.GetNewClientID();
        public static Task UpdateClient(LocalClientModel client, string userID) => Instance._databaseHelper.UpdateClient(client, userID);
        public static Task<LocalClientModel> GetClientInfo(string clientID) => Instance._databaseHelper.GetClientInfo(clientID);
        public static Task DeleteClient(string clientID) => Instance._databaseHelper.DeleteClient(clientID);
        public static Task<List<LocalClientModel>> GetUserClients(string userID) => Instance._databaseHelper.GetUserClients(userID);
        #endregion

        #region Users
        public static Task UpdateUser(UserModel user) => Instance._databaseHelper.UpdateUser(user);
        public static Task<UserModel> GetUserByEmail(string email) => Instance._databaseHelper.GetUser(email);
        public static Task<bool> AuthenticateUser(string email, string password) => Instance._databaseHelper.AuthenticateUser(email, password);
        #endregion

        #region ClientMessages
        public static Task InsertClientMessage(ClientMessageModel message) => Instance._databaseHelper.InsertClientMessage(message);
        public static Task DeleteClientMessage(ClientMessageModel message) => Instance._databaseHelper.DeleteClientMessage(message);
        public static Task<List<ClientMessageModel>> GetClientMessages(string clientID) => Instance._databaseHelper.GetClientMessages(clientID);
        #endregion

        #region ClientConfigs
        public static Task InsertClientConfig(SessionConfigModel config, string clientID) => Instance._databaseHelper.InsertClientConfig(config, clientID);
        public static Task<List<ClientConfigModel>> GetClientConfigs(string clientID) => Instance._databaseHelper.GetClientConfigs(clientID);
        #endregion

        DatabaseHelper _databaseHelper { get; set; }
        FTPHelper _ftpHelper { get; set; }
        VersionHelper _versionHelper { get; set; }

        List<MinerConfigModel> _allMiners { get; set; } = new List<MinerConfigModel>();

        string _minersJsonPath { get; set; } = Path.Combine(DataHelper.RootPath(), "Miners", "allminers.json");


        public ServerHelper()
        {
            Instance = this;

            _databaseHelper = new DatabaseHelper();            
            _ftpHelper = new FTPHelper();
            _versionHelper = new VersionHelper();

            _allMiners = GetMiners();
        }

        public string GetUpdateVersionString()
        {
            var path = "";

            _ftpHelper.DownloadFile(VersionHelper.CurrentVersionPath, out path);

            return File.ReadAllText(path);
        }

        public async Task<bool> CheckForUpdates()
        {
            var versionString = GetUpdateVersionString();

            _updateVersion = JsonConvert.DeserializeObject<VersionModel>(versionString);

            if (_updateVersion.Number > Bootstrapper.Settings.App.AppVersion.Number)
            {
                return true;
            }

            return false;
        }

        VersionModel _updateVersion = null;
        public VersionModel GetUpdateVersion(bool reload = false)
        {
            if (reload || _updateVersion == null)
            {
                _updateVersion = JsonConvert.DeserializeObject<VersionModel>(GetUpdateVersionString());
            }

            return _updateVersion;
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

        public static List<MinerConfigModel> GetMiners()
        {
            return new List<MinerConfigModel>()
            {
                new MinerConfigModel()
                {
                    Name = "CCMiner",
                    Cryptos = new List<string>() { "Vertcoin" },
                }
            };
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

        public class FTPHelper
        {
            FTPClient client { get; set; }

            static string _address = Path.Combine("ftp://23.229.226.104/");
            static string _username = "miningappclient@kodykriner.com";
            static string _password = "conquest.papal.tinny.redress.tuft";

            public FTPHelper()
            {
                client = new FTPClient(_address, _username, _password);
            }

            public void DownloadFile(string remoteFile, out string tmpPath)
            {
                try
                {
                    if (!Directory.Exists(Bootstrapper.AppTempPath))
                    {
                        Directory.CreateDirectory(Bootstrapper.AppTempPath);
                    }

                    var tmpID = Guid.NewGuid().ToString().Substring(0, 8);
                    tmpPath = Path.Combine(Bootstrapper.AppTempPath, tmpID);

                    client.download(remoteFile, tmpPath);
                }
                catch (Exception ex)
                {
                    LogHelper.AddEntry(ex);

                    tmpPath = string.Empty;
                }
            }

        }

        public class VersionHelper
        {
            public static string CurrentVersionPath = "version.info";

            public class VersionModel
            {
                public double Number { get; set; }

                public DateTime ReleaseTimestamp { get; set; }

                public string Notes { get; set; }

                public UrgencyType Urgency { get; set; }

                public enum UrgencyType : int
                {
                    Low = 0,
                    Minor = 1,
                    Major = 2,
                    Required = 3,
                }

                public VersionModel()
                {

                }
            }
        }
    }
}
