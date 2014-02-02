using System;

namespace P3bble.Core
{
    /// <summary>
    /// Raised when trying to install an app when there is no space
    /// </summary>
    public class CannotInstallException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotInstallException"/> class.
        /// </summary>
        public CannotInstallException()
            : base("There are no memory slots free")
        {
        }
    }
}
