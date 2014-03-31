using System;

namespace P3bble
{
    /// <summary>
    /// Raised when calling a method on an unconnected Pebble
    /// </summary>
    public class NotConnectedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotConnectedException"/> class.
        /// </summary>
        public NotConnectedException()
            : base("You first need to connect to the Pebble")
        {
        }
    }
}
