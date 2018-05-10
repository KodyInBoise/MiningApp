using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class UserModel
    {
        public List<string> CryptoWatchList { get; set; }

        public UserModel()
        {
            CryptoWatchList = new List<string>();
        }

        public void AddToCryptoWatchList(CryptoModel crypto)
        {
            if (!CryptoWatchList.Contains(crypto.Symbol))
            {
                CryptoWatchList.Add(crypto.Symbol);

                SaveSettings();
            }
        }

        private void SaveSettings()
        {
            DataHelper.SaveUserSettings(this);
        }
    }
}
