using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3bble.Core.Constants;
using P3bble.Core.Types;

namespace P3bble.Core.Messages
{
    /// <summary>
    /// Represents the type we're putting on the Pebble
    /// </summary>
    internal enum PutBytesTransferType : byte
    {
        /// <summary>
        /// Firmware content
        /// </summary>
        Firmware = 1,

        /// <summary>
        /// Recovery content
        /// </summary>
        Recovery = 2,

        /// <summary>
        /// System resources content
        /// </summary>
        SystemResources = 3,

        /// <summary>
        /// Resources content
        /// </summary>
        Resources = 4,

        /// <summary>
        /// Binary content
        /// </summary>
        Binary = 5
    }

    /// <summary>
    /// Represents the state
    /// </summary>
    internal enum PutBytesState : byte
    {
        /// <summary>
        /// The not started state
        /// </summary>
        NotStarted = 0,

        /// <summary>
        /// The wait for token state
        /// </summary>
        WaitForToken = 1,

        /// <summary>
        /// The in progress state
        /// </summary>
        InProgress = 2,

        /// <summary>
        /// The commit state
        /// </summary>
        Commit = 3,

        /// <summary>
        /// The complete state
        /// </summary>
        Complete = 4,

        /// <summary>
        /// The failed state
        /// </summary>
        Failed = 5
    }

    internal class PutBytesMessage : P3bbleMessage
    {
        private PutBytesTransferType _transferType;
        private byte[] _buffer;
        private uint _index;
        private bool _done;
        private bool _error;
        private PutBytesState _state;

        public PutBytesMessage()
            : base(P3bbleEndpoint.PutBytes)
        {
        }

        public PutBytesMessage(PutBytesTransferType transferType, byte[] buffer, uint index = 0)
            : base(P3bbleEndpoint.PutBytes)
        {
            this._transferType = transferType;
            this._buffer = buffer;
            this._index = index;
            this._state = PutBytesState.NotStarted;
        }

        internal bool HandleStateMessage(PutBytesMessage message)
        {
            return false;
        }

        protected override void AddContentToMessage(List<byte> payload)
        {
            switch (this._state)
            {
                case PutBytesState.NotStarted:
                    break;
            }
        }

        protected override void GetContentFromMessage(List<byte> payload)
        {
        }
    }
}
