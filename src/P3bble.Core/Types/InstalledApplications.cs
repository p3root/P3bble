using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Types
{
    /// <summary>
    /// Represents the installed applications
    /// </summary>
    public class InstalledApplications
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstalledApplications"/> class.
        /// </summary>
        /// <param name="banks">The banks.</param>
        internal InstalledApplications(uint banks)
        {
            this.ApplicationBanks = banks;
            this.ApplicationsInstalled = new List<InstalledApplication>();
        }

        /// <summary>
        /// Gets the number of application banks available.
        /// </summary>
        /// <value>
        /// The application banks.
        /// </value>
        public uint ApplicationBanks { get; private set; }

        /// <summary>
        /// Gets the applications installed.
        /// </summary>
        /// <value>
        /// The applications installed.
        /// </value>
        public List<InstalledApplication> ApplicationsInstalled { get; private set; }
    }
}
