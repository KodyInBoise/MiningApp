using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{

    public class SessionModel
    {
        public class Config
        {
            public int ID { get; set; }
            public int MinerID { get; set; }
            public int WalletID { get; set; }
            public int PoolID { get; set; }

            MinerConfigModel _miner = null;
            public MinerConfigModel GetMiner(bool reload = false)
            {
                if (_miner == null || reload)
                {
                    _miner = DataHelper.Instance.GetMinerByID(MinerID);
                }

                return _miner;
            }

            WalletConfigModel _wallet = null;
            public WalletConfigModel GetWallet(bool reload = false)
            {
                if (_wallet == null || reload)
                {
                    _wallet = DataHelper.Instance.GetWalletByID(WalletID);
                }

                return _wallet;
            }

            PoolConfigModel _pool = null;
            public PoolConfigModel GetPool(bool reload = false)
            {
                if (_pool == null || reload)
                {
                    _pool = DataHelper.Instance.GetPoolByID(PoolID);
                }

                return _pool;
            }
        }


        public string SessionID { get; set; }
        
        public Config SessionConfig { get; set; }

        public MinerConfigModel MinerConfig;

        public WalletConfigModel WalletConfig;

        public PoolConfigModel PoolConfig;

        public SessionModel(int configID)
        {
            SessionConfig = DataHelper.Instance.GetSessionConfigByID(configID);


        }
    }
}
