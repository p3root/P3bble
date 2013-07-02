using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Constants
{
    public enum P3bbleRemoteCaps : uint
    {
        UNKNOWN = 0,
        IOS = 1,
        ANDROID = 2,
        OSX = 3,
        LINUX = 4,
        WINDOWS = 5,
        TELEPHONY = 16,
        SMS = 32,
        GPS = 64,
        BTLE = 128,
        CAMERA_FRONT = 240,
        CAMERA_REAR = 256,
        ACCEL = 512,
        GYRO = 1024,
        COMPASS = 2048
    }
}
