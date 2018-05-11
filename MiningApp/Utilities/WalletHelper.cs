using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class WalletHelper
    {
        public static WalletHelper Instance { get; set; }

        public List<WalletConfigModel> AllWallets => DataHelper.Instance.GetWalletConfigs();

        public WalletHelper()
        {
            Instance = this;
        }
    }
}
