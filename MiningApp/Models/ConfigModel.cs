using LiteDB;
using MiningApp.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public enum MinerStatus
    {
        Inactive,
        Stopped,
        Running
    }

    public class ConfigModel
    {
        public int ConfigID { get; set; }

        public int MinerID { get; set; }

        public int WalletID { get; set; }

        public DateTime CreatedTimestamp { get; set; }

        public string Name { get; set; }

        public string CryptoName { get; set; }

        public string Arguments { get; set; }

        public string Output { get; set; } = "";

        public bool ShowWindow { get; set; } = true;
        
        public MinerConfigModel Miner { get; set; }

        public WalletConfigModel Wallet { get; set; }

        public PoolConfigModel Pool { get; set; }

        public double StaleOutputThreshold { get; set; }

        public MinerType MinerType { get; set; }

        [BsonIgnore]
        public MiningSessionModel Session { get; set; }


        public async Task SaveMinerSettings()
        {
            switch (MinerType)
            {
                case MinerType.CCMiner:
                    await MinerSettings.CCMiner.SaveParams(Pool.Address, Wallet.Address);
                    break;
                default:
                    await MinerSettings.CCMiner.SaveParams(Pool.Address, Wallet.Address);
                    break;
            }
        }

        public void StartSession()
        {
            Session = new MiningSessionModel(this);

            WindowController.MiningSessions.Add(Session);

            Task.Run(Session.Start);

            WindowController.Instance.ShowHome(Session);
        }
    }
}
