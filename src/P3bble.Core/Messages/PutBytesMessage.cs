using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3bble.Constants;
using P3bble.Helper;
using P3bble.Types;

namespace P3bble.Messages
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
    /// <remarks>The actual number values are important and are used for readability below in AddContentToMessage</remarks>
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
        /// The abort state
        /// </summary>
        Abort = 4,

        /// <summary>
        /// The complete state
        /// </summary>
        Complete = 5,

        /// <summary>
        /// The failed state
        /// </summary>
        Failed = 6,
    }

    internal class PutBytesMessage : P3bbleMessage
    {
        private PutBytesTransferType _transferType;
        private List<byte> _buffer;
        private int _leftToSend;
        private int _lastChunkSize;
        private uint _index;
        private uint _token;
        private PutBytesState _state;
        private List<byte> _result;
        private InstallProgressHandler _progressHandler;

        public PutBytesMessage()
            : base(Endpoint.PutBytes)
        {
        }

        public PutBytesMessage(PutBytesTransferType transferType, byte[] buffer, InstallProgressHandler progressHandler, uint index = 0)
            : base(Endpoint.PutBytes)
        {
            this._transferType = transferType;
            this._buffer = new List<byte>(buffer);
            this._progressHandler = progressHandler;
            this._index = index;
            this._state = PutBytesState.NotStarted;
        }

        internal bool Completed { get; set; }

        internal bool Errored { get; set; }

        internal List<byte> Result
        {
            get
            {
                return this._result;
            }
        }

        /// <summary>
        /// Handles the state message.
        /// </summary>
        /// <param name="message">The message response.</param>
        /// <returns>True when the PutBytes process has completed (either successfully or not), false if further processing is required</returns>
        internal bool HandleStateMessage(PutBytesMessage message)
        {
            if (message.Result[0] != 1)
            {
                this.Errored = true;
            }

            Logger.WriteLine("PutBytes >>> incoming message for " + this._state.ToString());

            switch (this._state)
            {
                case PutBytesState.WaitForToken:
                    if (this.Errored)
                    {
                        return true;
                    }

                    byte[] tokenArray = new byte[message.Result.Count - 1];
                    message.Result.CopyTo(1, tokenArray, 0, tokenArray.Length);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(tokenArray);
                    }

                    this._token = BitConverter.ToUInt32(tokenArray, 0);

                    this._leftToSend = this._buffer.Count;

                    // next step is we start sending data - like PutBytesClient.send()
                    // the continuation is in AddContentToMessage
                    this._state = PutBytesState.InProgress;
                    break;

                case PutBytesState.InProgress:
                    if (this.Errored)
                    {
                        this._state = PutBytesState.Abort;
                    }
                    else
                    {
                        if (this._leftToSend > 0)
                        {
                            // Still more to send, so we return false and the next chunk will go
                            Logger.WriteLine(string.Format("Sent {0} of {1} bytes", this._buffer.Count - this._leftToSend, this._buffer.Count));
                            if (this._progressHandler != null)
                            {
                                // From PutBytes, we send the number of bytes we just sent rather than a percentage...
                                this._progressHandler(this._lastChunkSize);
                            }

                            return false;
                        }
                        else
                        {
                            // next step is we send the commit message like PutBytesClient.commit()
                            // the continuation is in AddContentToMessage
                            this._state = PutBytesState.Commit;
                        }
                    }

                    break;

                case PutBytesState.Commit:
                    if (this.Errored)
                    {
                        this._state = PutBytesState.Abort;
                    }
                    else
                    {
                        // next step is we send the commit message like PutBytesClient.commit()
                        // the continuation is in AddContentToMessage
                        this._state = PutBytesState.Complete;
                    }

                    break;

                case PutBytesState.Complete:
                    if (this.Errored)
                    {
                        this._state = PutBytesState.Abort;
                    }
                    else
                    {
                        this.Completed = true;
                        return true;
                    }

                    break;
            }

            return false;
        }

        protected override void AddContentToMessage(List<byte> payload)
        {
            Logger.WriteLine("PutBytes <<< outgoing message for " + this._state.ToString());

            switch (this._state)
            {
                case PutBytesState.NotStarted:
                    {
                        // another magic number!...
                        payload.Add((byte)PutBytesState.WaitForToken);

                        byte[] length = BitConverter.GetBytes(this._buffer.Count);
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(length);
                        }

                        payload.AddRange(length);
                        payload.Add((byte)this._transferType);
                        payload.Add((byte)this._index);

                        // Move to next state...
                        this._state = PutBytesState.WaitForToken;
                    }

                    break;

                case PutBytesState.InProgress:
                    {
                        // equivalent of python PutBytesClient.send()
                        this._lastChunkSize = Math.Min(this._leftToSend, 2000);
                        int offset = this._buffer.Count - this._leftToSend;

                        // another magic number!...
                        payload.Add((byte)this._state);

                        byte[] tokenBytes = BitConverter.GetBytes(this._token & 0xFFFFFFFF);
                        byte[] dataToSendBytes = BitConverter.GetBytes(this._lastChunkSize);

                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(tokenBytes);
                            Array.Reverse(dataToSendBytes);
                        }

                        payload.AddRange(tokenBytes);
                        payload.AddRange(dataToSendBytes);

                        byte[] data = new byte[this._lastChunkSize];
                        this._buffer.CopyTo(offset, data, 0, this._lastChunkSize);
                        payload.AddRange(data);
                        this._leftToSend -= this._lastChunkSize;

                        Logger.WriteLine("PutBytes - sending " + this._lastChunkSize.ToString() + " byte(s), " + this._leftToSend.ToString() + " byte(s) remain");
                    }

                    break;

                case PutBytesState.Abort:
                    {
                        // equivalent of python PutBytesClient.abort()
                        payload.Add((byte)this._state);
                        byte[] tokenBytes = BitConverter.GetBytes(this._token & 0xFFFFFFFF);

                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(tokenBytes);
                        }

                        payload.AddRange(tokenBytes);
                    }

                    break;

                case PutBytesState.Commit:
                    {
                        payload.Add((byte)this._state);
                        byte[] tokenBytes = BitConverter.GetBytes(this._token & 0xFFFFFFFF);

                        uint crc = this._buffer.Crc32();
                        byte[] crcBytes = BitConverter.GetBytes(crc);
                        Logger.WriteLine(string.Format("Sending CRC of {0:X}", crc));

                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(tokenBytes);
                            Array.Reverse(crcBytes);
                        }

                        payload.AddRange(tokenBytes);
                        payload.AddRange(crcBytes);
                    }

                    break;

                case PutBytesState.Complete:
                    {
                        payload.Add((byte)this._state);
                        byte[] tokenBytes = BitConverter.GetBytes(this._token & 0xFFFFFFFF);

                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(tokenBytes);
                        }

                        payload.AddRange(tokenBytes);
                    }

                    break;
            }
        }

        protected override void GetContentFromMessage(List<byte> payload)
        {
            this._result = payload;
        }
    }
}
