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

    public enum LogType : int
    {
        General = 0, //App startup, shutdown, config / setting changes, etc
        Error = 1, 
        Session = 2, //Start, stop, restarts, etc
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
}
