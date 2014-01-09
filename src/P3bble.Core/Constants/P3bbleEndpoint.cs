using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Constants
{
    internal enum P3bbleEndpoint
    {
        Firmware = 1,
        Time = 11,
        Version = 16,
        /// <summary>
        /// Pebble is requesting details about the phone version
        /// </summary>
        PhoneVersion = 17,
        SystemMessage = 18,
        MusicControl = 32,
        PhoneControl = 33,
        ApplicationMessage = 48,
        Launcher = 49,
        Logs = 2000,
        Ping = 2001,
        LogsDump = 2002,
        Reset = 2003,
        //AppMfg = 2004, - seems unused
        AppLogs = 2006,
        Notification = 3000,
        Resource = 4000,
        SysReg = 5000,
        FctReg = 5001,
        AppManager = 6000,
        RunKeeper = 7000,
        PutBytes = 48879
    }
}
