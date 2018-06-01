﻿using System;
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

    public enum MinerType
    {
        Empty,
        CCMiner,
    }

    public enum MiningAlgorithm
    {

    }

    public enum ButtonStyle
    {
        Normal,
        New,
        Delete,
        Next,
        Finish,
    }

    public enum LogType : int
    {
        General = 0, //App startup, shutdown, config / setting changes, etc
        Error = 1, 
        Session = 2, //Start, stop, restarts, etc
    }
}
