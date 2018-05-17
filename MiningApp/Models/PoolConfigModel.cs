using LiteDB;
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

        public DateTime CreatedTimestamp { get; set; }

        public string Name { get; set; }

        public string Crypto { get; set; }

        public string Address { get; set; }

        public double Fee { get; set; }

        public string Note { get; set; }

        public double Rating { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        [BsonIgnore]
        public string FeeString => $"{Fee}%";

        

        public PoolConfigModel()
        {

        }

        public override string ToString()
        {
            return Name;
        }
    }
}
