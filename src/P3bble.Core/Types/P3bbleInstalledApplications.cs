using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Types
{
    /// <summary>
    /// Represents the installed applications
    /// </summary>
    public class P3bbleInstalledApplications
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="P3bbleInstalledApplications"/> class.
        /// </summary>
        /// <param name="banks">The banks.</param>
        internal P3bbleInstalledApplications(uint banks)
        {
            this.ApplicationBanks = banks;
            this.ApplicationsInstalled = new List<P3bbleInstalledApplication>();
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
        public List<P3bbleInstalledApplication> ApplicationsInstalled { get; private set; }
    }
}
