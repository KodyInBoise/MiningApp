using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public static class MinerNames
    {
        public static MinerType GetTypeByName(string name)
        {
            if (name == MinerSettings.CCMiner.MinerName)
            {
                return MinerType.CCMiner;
            }

            return MinerType.Empty;
        }
    }
}
