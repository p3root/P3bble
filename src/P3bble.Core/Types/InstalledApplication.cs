namespace P3bble.Types
{
    /// <summary>
    /// Represents an installed application
    /// </summary>
    public class InstalledApplication
    {
        /// <summary>
        /// Gets the application id
        /// </summary>
        public uint Id { get; internal set; }

        /// <summary>
        /// Gets the application index
        /// </summary>
        public uint Index { get; internal set; }

        /// <summary>
        /// Gets the name
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the company
        /// </summary>
        public string Company { get; internal set; }

        /// <summary>
        /// Gets the flags
        /// </summary>
        public uint Flags { get; internal set; }

        /// <summary>
        /// Gets the version
        /// </summary>
        public ushort Version { get; internal set; }
    }
}
