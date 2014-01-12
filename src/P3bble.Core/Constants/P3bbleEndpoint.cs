using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Constants
{
    /// <summary>
    /// The endpoint
    /// </summary>
    internal enum P3bbleEndpoint
    {
        /// <summary>
        /// Firmware endpoint
        /// </summary>
        Firmware = 1,

        /// <summary>
        /// Time endpoint
        /// </summary>
        Time = 11,

        /// <summary>
        /// Version endpoint
        /// </summary>
        Version = 16,

        /// <summary>
        /// Pebble is requesting details about the phone version
        /// </summary>
        PhoneVersion = 17,

        /// <summary>
        /// The system message endpoint
        /// </summary>
        SystemMessage = 18,

        /// <summary>
        /// The music control endpoint
        /// </summary>
        MusicControl = 32,

        /// <summary>
        /// The phone control endpoint
        /// </summary>
        PhoneControl = 33,

        /// <summary>
        /// The application message endpoint
        /// </summary>
        ApplicationMessage = 48,

        /// <summary>
        /// The launcher endpoint
        /// </summary>
        Launcher = 49,

        /// <summary>
        /// The logs endpoint
        /// </summary>
        Logs = 2000,

        /// <summary>
        /// The ping endpoint
        /// </summary>
        Ping = 2001,

        /// <summary>
        /// The logs dump endpoint
        /// </summary>
        LogsDump = 2002,

        /// <summary>
        /// The reset endpoint
        /// </summary>
        Reset = 2003,
        
        ////AppMfg = 2004, - seems unused

        /// <summary>
        /// The application logs endpoint
        /// </summary>
        AppLogs = 2006,

        /// <summary>
        /// The notification endpoint
        /// </summary>
        Notification = 3000,

        /// <summary>
        /// The resource endpoint
        /// </summary>
        Resource = 4000,

        /// <summary>
        /// The system reg endpoint
        /// </summary>
        SysReg = 5000,

        /// <summary>
        /// The FCT reg endpoint
        /// </summary>
        FctReg = 5001,

        /// <summary>
        /// The application manager endpoint
        /// </summary>
        AppManager = 6000,

        /// <summary>
        /// The run keeper endpoint
        /// </summary>
        RunKeeper = 7000,

        /// <summary>
        /// The put bytes endpoint
        /// </summary>
        PutBytes = 48879
    }
}
