using System;
using System.Runtime.Serialization;
using P3bble.Helper;

namespace P3bble.Types
{
    /// <summary>
    /// Details about a firmware release
    /// </summary>
    [DataContract]
    public class FirmwareRelease : VersionInfo
    {
        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        [DataMember(Name = "url", IsRequired = true)]
        public string Url { get; private set; }

        /// <summary>
        /// Gets the notes.
        /// </summary>
        /// <value>
        /// The notes.
        /// </value>
        [DataMember(Name = "notes", IsRequired = true)]
        public string Notes { get; private set; }

        /// <summary>
        /// Gets the checksum.
        /// </summary>
        /// <value>
        /// The checksum.
        /// </value>
        [DataMember(Name = "sha-256", IsRequired = true)]
        public string Checksum { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            string format = "Version: {0}\n"
                          + "Url:     {1}\n"
                          + "Notes:   {2}\n";
            return string.Format(format, this.Version, this.Url, this.Notes);
        }
    }
}
