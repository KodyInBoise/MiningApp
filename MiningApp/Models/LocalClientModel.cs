using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class LocalClientModel
    {
        public string ID { get; set; }

        public DateTime LastCheckin { get; set; }

        bool _useServer => Bootstrapper.Settings.General.UseServer;


        public LocalClientModel()
        {
            if (String.IsNullOrEmpty(Bootstrapper.Settings.Server.LocalClientID))
            {
                Bootstrapper.Settings.Server.LocalClientID = ElementHelper.GetNewGuid(8);
                Bootstrapper.Instance.SaveLocalSettings();
            }

            ID = Bootstrapper.Settings.Server.LocalClientID;
        }

        public static async void Test()
        {
            await ServerHelper.Instance.UpdateClient;
        }
    }
}
