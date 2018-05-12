using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class PoolHelper
    {
        public static PoolHelper Instance { get; set; }

        public List<PoolConfigModel> LocalPools => DataHelper.Instance.GetAllPools();

        public List<PoolConfigModel> AllPools { get; set; } = new List<PoolConfigModel>();

        public PoolHelper()
        {
            Instance = this;
        }

        public bool PoolNameTaken(string name)
        {
            var wallet = LocalPools.Find(x => x.Name == name);

            return wallet == null ? false : true;
        }
    }
}
