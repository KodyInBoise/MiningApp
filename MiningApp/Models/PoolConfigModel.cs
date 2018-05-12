using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class PoolConfigModel
    {
        public int ID { get; set; }

        public DateTime AddedTimestamp { get; set; }

        public string Name { get; set; }

        public string Crypto { get; set; }

        public string Address { get; set; }

        public double Fee { get; set; }

        public string Note { get; set; }

        public double Rating { get; set; }
    }
}
