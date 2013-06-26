using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core
{
    public enum P3bbleState
    {
        Connected,
        Connecting,
        Disconnected,
        Disconnecting,
        NoPebbleFound,
        Unkown
    }
}
