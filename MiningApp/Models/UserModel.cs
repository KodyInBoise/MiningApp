using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class UserModel
    {
        public double? ControlBarLeft { get; set; }

        public double? ControlBarTop { get; set; }

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

        public void SaveSettings()
        {
            GetWindowLocation();

            DataHelper.SaveUserSettings(this);
        }

        private void GetWindowLocation()
        {
            ControlBarLeft = WindowController.Instance.ControlBarWin.Left;
            ControlBarTop = WindowController.Instance.ControlBarWin.Top;
        }
    }
}
