using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core
{
    public enum P3bbleEndpoint
    {
        Firmware = 1,
        Time = 11,
        Version = 16,
        PhoneVersion = 17,
        SystemMessage = 18,
        MusicControl = 32,
        PhoneControl = 33,
        Logs = 2000,
        Ping = 2001,
        LogsDump = 2002,
        Reset = 2003,
        AppMfg = 2004,
        Notification = 3000,
        Resource = 4000,
        SysReg = 5000,
        FctReg = 5001,
        AppInstallManager = 6000,
        RunKeeper = 7000,
        PutBytes = 48879
    }
}
