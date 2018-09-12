using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class LocalClientModel
    {
        public static LocalClientModel Instance { get; set; }


        public string ID { get; set; }

        public DateTime LastCheckin { get; set; }


        TimerModel _checkinTimer { get; set; }

        int _checkinInterval { get; set; } = 15;

        bool _useServer => Bootstrapper.Settings.Server.UseServer;


        public LocalClientModel()
        {
            Instance = this;

            if (String.IsNullOrEmpty(Bootstrapper.Settings.LocalClient.LocalClientID))
            {
                Bootstrapper.Settings.LocalClient.LocalClientID = ElementHelper.GetNewGuid(8);
                Bootstrapper.SaveLocalSettings();
            }

            ID = Bootstrapper.Settings.LocalClient.LocalClientID;

            _checkinTimer = new TimerModel(this, PerformCheckin, interval: _checkinInterval);
        }

        public static async void PerformCheckin()
        {
            try
            {
                if (Bootstrapper.Settings.Server.UseServer && Bootstrapper.Settings.Server.UserAuthenticated)
                {
                    await Task.Run(() => 
                    ServerHelper.UpdateClient(Bootstrapper.Settings.LocalClient.LocalClientID, Bootstrapper.Settings.User.UserID));
                }
            }
            catch (Exception ex)
            {
                ExceptionUtil.Handle(ex);
            }
        }

        public static async void Test()
        {
            var user = new UserModel()
            {
                ID = ElementHelper.GetNewGuid(8),
                Email = "kody.kriner@gmail.com",
                Password = "password",
            };

            await ServerHelper.UpdateUser(user);
        }
    }
}
