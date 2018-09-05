using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class LocalClientModel
    {
        public string ID => Bootstrapper.Settings.Server.LocalClientID;

        public DateTime LastConnect { get; set; }

        bool _useServer => Bootstrapper.Settings.General.UseServer;


        public LocalClientModel()
        {
            if (String.IsNullOrEmpty(ID))
            {
                Bootstrapper.Settings.Server.LocalClientID = ServerHelper.Instance.NewClientID;
                Bootstrapper.Instance.SaveLocalSettings();
            }
        }

        public static async void Test()
        {
            await ServerHelper.Instance.UpdateClient;
        }
    }
}
