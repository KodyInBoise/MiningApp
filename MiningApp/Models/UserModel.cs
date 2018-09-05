using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class UserModel
    {
        public string ID { get; set; }

        public string Email { get; set; }

        public DateTime LastServerLogin { get; set; }

        public List<string> WatchingCryptos { get; set; }

        public UserModel()
        {
            WatchingCryptos = new List<string>();
            ID = Bootstrapper.Settings.User.UserID;

            if (String.IsNullOrEmpty(ID))
            {
                Bootstrapper.Settings.User.UserID = ElementHelper.GetNewGuid(8);
                Bootstrapper.Instance.SaveLocalSettings();
            }
        }

        string CreateNewUserID()
        {
            return Guid.NewGuid().ToString();
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
            DataHelper.SaveUserSettings(this);
        }
    }
}
