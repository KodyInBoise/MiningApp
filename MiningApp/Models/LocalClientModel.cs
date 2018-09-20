using MiningApp.LoggingUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public delegate void ClientMessageReceivedDelegate(ClientMessageReceivedArgs args);

    public class ClientMessageReceivedArgs
    {
        public DateTime Timestamp { get; set; }
        public List<ClientMessageModel> Messages { get; set; }

        public ClientMessageReceivedArgs()
        {
            Timestamp = DateTime.Now;
        }
    }

    public class ServerClientModel
    {
        public static ServerClientModel Instance { get; private set; }

        public static ClientMessageReceivedDelegate MessageReceivedDelegate { get; private set; }


        public string ID { get; set; }

        public DateTime LastCheckin { get; set; }

        public string PublicIP { get; set; }

        public string PrivateIP { get; set; }


        TimerModel _checkinTimer { get; set; }

        int _checkinInterval { get; set; } = 15;

        bool _useServer => Bootstrapper.Settings.Server.UseServer;


        public ServerClientModel()
        {
            Instance = this;

            if (String.IsNullOrEmpty(Bootstrapper.Settings.LocalClient.LocalClientID))
            {
                Bootstrapper.Settings.LocalClient.LocalClientID = ElementHelper.GetNewGuid(8);
                Bootstrapper.SaveLocalSettings();
            }

            ID = Bootstrapper.Settings.LocalClient.LocalClientID;

            _checkinTimer = new TimerModel(this, PerformCheckin, interval: _checkinInterval);

            Bootstrapper.UserAuthenticationDelegate += UserAuthenticationChanged;
            MessageReceivedDelegate += MessageReceived_Invoked;

            // TESTING
            PrivateIP = GetPrivateIP();
            PublicIP = GetPublicIP();
        }

        void UserAuthenticationChanged(UserAuthenticationChangedArgs args)
        {
            if (args.Status == UserAuthenticationStatus.Connected)
            {
                PerformCheckin();
            }
        }

        public static async void PerformCheckin()
        {
            try
            {
                if (Bootstrapper.Settings.Server.UseServer && Bootstrapper.Settings.Server.UserAuthenticated)
                {
                    await Task.Run(() => 
                    ServerHelper.UpdateClient(Instance, Bootstrapper.Settings.User.UserID));

                    var messages = await Task.Run(() => ServerHelper.GetClientMessages(Instance.ID));
                    if (messages.Any())
                    {
                        MessageReceivedDelegate?.Invoke(new ClientMessageReceivedArgs() { Messages = messages });
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionUtil.Handle(ex);
            }
        }

        void MessageReceived_Invoked(ClientMessageReceivedArgs args)
        {
            foreach (var message in args.Messages)
            {
                try
                {
                    LogHelper.AddEntry(LogType.Server, $"Received message from server: {message}");
                }
                catch (Exception ex)
                {
                    ExceptionUtil.Handle(ex);
                }
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

        public static string GetPrivateIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            return host.AddressList[0].MapToIPv4().ToString();
        }

        public static string GetPublicIP()
        {
            try
            {
                string ip = "";

                string url = "http://checkip.dyndns.org";
                WebRequest req = System.Net.WebRequest.Create(url);
                WebResponse resp = req.GetResponse();

                using (var rdr = new StreamReader(resp.GetResponseStream()))
                {
                    var response = rdr.ReadToEnd().Trim();

                    var part1 = response.Split(':');
                    var part2 = part1[1].Substring(1);
                    var part3 = part2.Split('<');

                    ip = part3[0];
                };


                return ip;
            }
            catch (Exception ex)
            {
                ExceptionUtil.Handle(ex);

                return "";
            }
        }
    }
}
