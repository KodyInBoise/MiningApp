using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class UserModel
    {
        public List<string> WatchingCryptos { get; set; }

        public UserModel()
        {
            WatchingCryptos = new List<string>();
        }

        public void AddToCryptoWatchList(CryptoModel crypto)
        {
            if (!WatchingCryptos.Contains(crypto.Name))
            {
                WatchingCryptos.Add(crypto.Name);

                SaveSettings();
            }
        }

        private void SaveSettings()
        {
            DataHelper.SaveUserSettings(this);
        }
    }
}
