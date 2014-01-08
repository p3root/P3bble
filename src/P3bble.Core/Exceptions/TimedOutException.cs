using System;

namespace P3bble.Core
{
    /// <summary>
    /// Raised when calling a method that expects a response, but doesn't receive one
    /// </summary>
    public class TimedOutException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimedOutException"/> class.
        /// </summary>
        public TimedOutException()
            : base("The operation timed out")
        {
        }
    }
}
