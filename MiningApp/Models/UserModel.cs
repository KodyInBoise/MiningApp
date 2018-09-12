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

        public string Password { get; set; }

        public bool RequiresLogin { get; set; }

        public DateTime LastServerLogin { get; set; }

        public List<string> WatchingCryptos { get; set; }

        public List<LocalClientModel> AllClients { get; set; }

        public UserModel()
        {
            WatchingCryptos = new List<string>();
            ID = Bootstrapper.Settings.User.UserID;

            Bootstrapper.UserAuthenticationDelegate += UserAuthenticationChanged;
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

        async void UserAuthenticationChanged(UserAuthenticationChangedArgs args)
        {
            if (args.Status == UserAuthenticationStatus.Connected)
            {
                try
                {
                    AllClients = await Task.Run(() => ServerHelper.GetUserClients(ID));
                }
                catch (Exception ex) { ExceptionUtil.Handle(ex); }
            }
        }
    }
}
