using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using P3bble.Constants;

namespace P3bble.Messages
{
    /// <summary>
    /// System command
    /// </summary>
    internal enum SystemCommand : byte
    {
        /// <summary>
        /// The firmware available command
        /// </summary>
        FirmwareAvailable = 0,

        /// <summary>
        /// The firmware start command
        /// </summary>
        FirmwareStart = 1,
        
        /// <summary>
        /// The firmware complete command
        /// </summary>
        FirmwareComplete = 2,
        
        /// <summary>
        /// The firmware fail command
        /// </summary>
        FirmwareFail = 3,
        
        /// <summary>
        /// The firmware up to date command
        /// </summary>
        FirmwareUpToDate = 4,
        
        /// <summary>
        /// The firmware out of date command
        /// </summary>
        FirmwareOutOfDate = 5,
        
        /// <summary>
        /// The bluetooth start discoverable command
        /// </summary>
        BluetoothStartDiscoverable = 6,
        
        /// <summary>
        /// The bluetooth end discoverable command
        /// </summary>
        BluetoothEndDiscoverable = 7
    }

    /// <summary>
    /// The System Message
    /// <remarks>
    /// These messages are used to signal important events/state-changes to the watch firmware.
    /// </remarks>
    /// </summary>
    internal class SystemMessage : P3bbleMessage
    {
        public SystemMessage()
            : base(Endpoint.SystemMessage)
        {
        }

        public SystemMessage(SystemCommand command)
            : this()
        {
            this.Command = command;
        }

        /// <summary>
        /// Gets the system command.
        /// </summary>
        /// <value>
        /// The system command.
        /// </value>
        public SystemCommand Command { get; private set; }

        protected override void AddContentToMessage(List<byte> payload)
        {
            payload.Add(0);

            // Add the command
            payload.Add((byte)this.Command);
        }

        protected override void GetContentFromMessage(List<byte> payload)
        {
            this.Command = (SystemCommand)payload[1];
        }
    }
}
