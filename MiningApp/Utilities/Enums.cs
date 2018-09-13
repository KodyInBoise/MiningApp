using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public enum SupportedCryptos
    {
        VTC,
    }

    public enum SessionType
    {
        Internal = 0,
        External = 1,
    }

    public enum MinerType
    {
        Empty,
        Internal,
        CCMiner,
    }

    public enum MinerStatus
    {
        Inactive,
        Stopped,
        Running
    }

    public enum MiningAlgorithm
    {

    }

    public enum ButtonStyleEnum
    {
        Normal,
        New,
        Delete,
        Next,
        Finish,
        Yellow,
        Orange,
    }

    public enum SessionStatusEnum : int
    {
        Stopped = 0,
        Running = 1,
        ManuallyPaused = 2,
        BlacklistPaused = 3
    }

    public enum BlacklistedItemType : int
    {
        Executable = 0,
        Directory = 1,
        Exclude = 2,
    }

    public class Enums
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
