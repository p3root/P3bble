using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P3bble.PCL.Logger
{
    public class BaseLogger
    {

        public virtual void WriteLine(string message) { }
        public virtual void ClearUp() { }

        /// <summary>
        /// Gets or sets a value indicating whether logging is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if logging is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled { get; set; }
    }
}
