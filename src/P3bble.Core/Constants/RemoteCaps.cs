using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Constants
{
    /// <summary>
    /// The phone capabilites
    /// </summary>
    [Flags]
    internal enum RemoteCaps : uint
    {
        /// <summary>
        /// Unknown capability
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// IOS platform
        /// </summary>
        IOS = 1,

        /// <summary>
        /// Android platform
        /// </summary>
        Android = 2,

        /// <summary>
        /// OSX platform
        /// </summary>
        OSX = 3,

        /// <summary>
        /// Linux platform
        /// </summary>
        Linux = 4,

        /// <summary>
        /// Windows platform
        /// </summary>
        Windows = 5,

        /// <summary>
        /// The telephony capability
        /// </summary>
        Telephony = 16,

        /// <summary>
        /// The SMS capability
        /// </summary>
        Sms = 32,

        /// <summary>
        /// The GPS capability
        /// </summary>
        Gps = 64,

        /// <summary>
        /// The BTLE capability
        /// </summary>
        BTLE = 128,

        /// <summary>
        /// The front camera capability
        /// </summary>
        CameraFront = 240,

        /// <summary>
        /// The rear camera capability
        /// </summary>
        CameraRear = 256,

        /// <summary>
        /// The acceleromter capability
        /// </summary>
        Acceleromter = 512,

        /// <summary>
        /// The gyroscope capability
        /// </summary>
        Gyroscope = 1024,

        /// <summary>
        /// The compass capability
        /// </summary>
        Compass = 2048
    }
}
