using P3bble.PCL.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core
{
    /// </summary>
    public class Logger : BaseLogger
    {
        private string FileName { get; set; }

        /// <summary>
        /// Writes a line to the debug log.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void WriteLine(string message)
        {
            Debug.WriteLine(message);
        }

        /// <summary>
        /// Clears up the current log file.
        /// </summary>
        public override void ClearUp()
        {
            
        }
    }
}
