using System;
using P3bble.Messages;

namespace P3bble
{
    /// <summary>
    /// Raised when there's a protocol error
    /// </summary>
    public class ProtocolException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolException"/> class.
        /// </summary>
        /// <param name="logMessage">The log message.</param>
        internal ProtocolException(LogsMessage logMessage)
            : base(logMessage.Message)
        {
            this.LogMessage = logMessage;
        }

        internal LogsMessage LogMessage { get; set; }
    }
}
