using P3bble.Core.Firmware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.EventArguments
{
    public class CheckForNewFirmwareVersionEventArgs : EventArgs
    {
        public bool NewVersionAvailable { get; private set; }
        public P3bbleFirmwareLatest NewVersionInfo { get; set; }

        public CheckForNewFirmwareVersionEventArgs(bool newVersionAvail, P3bbleFirmwareLatest newVersionInfo)
        {
            NewVersionAvailable = newVersionAvail;
            NewVersionInfo = newVersionInfo;
        }
    }
}
