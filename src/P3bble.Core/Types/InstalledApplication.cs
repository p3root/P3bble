namespace P3bble.Types
{
    /// <summary>
    /// Represents an installed application
    /// </summary>
    public struct InstalledApplication
    {
        /// <summary>
        /// The application id
        /// </summary>
        public uint Id;

        /// <summary>
        /// The application index
        /// </summary>
        public uint Index;

        /// <summary>
        /// The name
        /// </summary>
        public string Name;

        /// <summary>
        /// The company
        /// </summary>
        public string Company;

        /// <summary>
        /// The flags
        /// </summary>
        public uint Flags;

        /// <summary>
        /// The version
        /// </summary>
        public ushort Version;
    }
}
