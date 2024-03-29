﻿using LiteDB;
using MiningApp.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class SessionConfigModel
    {
        public int ID { get; set; }

        public SessionType Type { get; set; }

        public int MinerID { get; set; }

        public int WalletID { get; set; }

        public int PoolID { get; set; }

        public DateTime CreatedTimestamp { get; set; }

        public string Name { get; set; }

        public string CryptoName { get; set; }

        public string Arguments { get; set; }

        public bool ShowWindow { get; set; } = true;

        public double StaleOutputThreshold { get; set; }

        public MinerType InternalMinerType { get; set; }


        [BsonIgnore]
        public MinerConfigModel Miner { get; set; }

        [BsonIgnore]
        public WalletConfigModel Wallet { get; set; }

        [BsonIgnore]
        public PoolConfigModel Pool { get; set; }

        [BsonIgnore]
        public SessionModel Session { get; set; }


        public async Task SaveMinerSettings()
        {
            switch (InternalMinerType)
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
            var session = WindowController.MiningSessions.Find(x => x.Config.ID == ID);

            Session = session ?? new SessionModel(this);
            WindowController.MiningSessions.Add(Session);

            Session.ToggleStatus(SessionStatusEnum.Running);

            WindowController.Instance.ShowHome(Session);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
