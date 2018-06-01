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

        public List<WalletConfigModel> AllWallets => DataHelper.Instance.GetWallets().Result;

        public WalletHelper()
        {
            Instance = this;
        }

        public bool WalletNameTaken(string name)
        {
            var wallet = AllWallets.Find(x => x.Name == name);

            return wallet == null ? false : true;
        }
    }
}
