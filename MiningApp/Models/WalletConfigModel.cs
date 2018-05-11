using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class WalletConfigModel
    {
        public int ID { get; set; }

        public DateTime CreatedTimestamp { get; set; }

        public string Name { get; set; }

        public string Crypto { get; set; }

        public string Address { get; set; }

        public string ClientPath { get; set; }

        public WalletConfigModel()
        {

        }

        public override string ToString()
        {
            return Name;
        }
    }
}
