using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class ClientConfigModel : SessionConfigModel
    {
        public string ClientID { get; set; }

        public string MinerName { get; set; }

        public int Status { get; set; }


        public ClientConfigModel()
        {

        }

        public ClientConfigModel(SessionConfigModel config, string clientID)
        {
            ServerID = config.ServerID;
            ClientID = clientID;
            Name = config.Name;
            MinerName = config.Miner.Name;
            CryptoName = config.CryptoName;
            Status = config.Session == null ? 0 : (int)config.Session.CurrentStatus;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
