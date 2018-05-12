using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    class MiningHelper
    {
        public static MiningHelper Instance { get; set; }

        public List<MiningRuleModel> LocalMiners => DataHelper.Instance.GetAllMiners().Result;

        public List<MinerConfigModel> AllMiners => ServerHelper.Instance.GetMiners();

        public MiningHelper()
        {
            Instance = this;
        }

    }
}
