using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Messages
{
    /// <summary>
    /// Represents the type we're putting on the Pebble
    /// </summary>
    public enum PutBytesType : byte
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
    public enum PutBytesState
    {
        /// <summary>
        /// The not started state
        /// </summary>
        NotStarted,

        /// <summary>
        /// The wait for token state
        /// </summary>
        WaitForToken,

        /// <summary>
        /// The in progress state
        /// </summary>
        InProgress,

        /// <summary>
        /// The commit state
        /// </summary>
        Commit,

        /// <summary>
        /// The complete state
        /// </summary>
        Complete,

        /// <summary>
        /// The failed state
        /// </summary>
        Failed
    }

    internal class PutBytesMessage : P3bbleMessage
    {
        public PutBytesMessage(PutBytesType type)
            : base(Constants.P3bbleEndpoint.PutBytes)
        {
        }

        protected override ushort PayloadLength
        {
            get
            {
                return base.PayloadLength;
            }
        }

        protected override void AddContentToMessage(List<byte> payload)
        {
            base.AddContentToMessage(payload);
        }

        protected override void GetContentFromMessage(List<byte> payload)
        {
            base.GetContentFromMessage(payload);
        }
    }
}
