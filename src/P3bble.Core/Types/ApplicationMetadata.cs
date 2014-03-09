using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace P3bble.Types
{
    /// <summary>
    /// Represents details of an application
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "It's a structure deserialized from a stream")]
    public struct ApplicationMetadata
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        internal readonly string Header;
        [MarshalAs(UnmanagedType.U1)]
        internal readonly byte StructMajorVersion;
        [MarshalAs(UnmanagedType.U1)]
        internal readonly byte StructMinorVersion;
        [MarshalAs(UnmanagedType.U1)]
        internal readonly byte SdkMajorVersion;
        [MarshalAs(UnmanagedType.U1)]
        internal readonly byte SdkMinorVersion;
        [MarshalAs(UnmanagedType.U1)]
        internal readonly byte AppMajorVersion;
        [MarshalAs(UnmanagedType.U1)]
        internal readonly byte AppMinorVersion;
        [MarshalAs(UnmanagedType.U2)]
        internal readonly ushort Size;
        [MarshalAs(UnmanagedType.U4)]
        internal readonly uint Offset;
        [MarshalAs(UnmanagedType.U4)]
        internal readonly uint CRC;

        /// <summary>
        /// The application name
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public readonly string AppName;

        /// <summary>
        /// The company name
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public readonly string CompanyName;
        
        [MarshalAs(UnmanagedType.U4)]
        internal readonly uint IconResourceID;
        [MarshalAs(UnmanagedType.U4)]
        internal readonly uint SymbolTableAddress;
        [MarshalAs(UnmanagedType.U4)]
        internal readonly uint Flags;
        [MarshalAs(UnmanagedType.U4)]
        internal readonly uint RelocationListStart;
        [MarshalAs(UnmanagedType.U4)]
        internal readonly uint RelocationListItemCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        internal readonly byte[] UuidInternal;

        internal Version StructVersion
        {
            get
            {
                return new Version(string.Format("{0}.{1}", this.StructMajorVersion, this.StructMinorVersion));
            }
        }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <value>
        /// The application version.
        /// </value>
        public Version AppVersion
        {
            get
            {
                return new Version(string.Format("{0}.{1}", this.AppMajorVersion, this.AppMinorVersion));
            }
        }

        /// <summary>
        /// Gets the SDK version.
        /// </summary>
        /// <value>
        /// The SDK version.
        /// </value>
        public Version SdkVersion
        {
            get
            {
                return new Version(string.Format("{0}.{1}", this.SdkMajorVersion, this.SdkMinorVersion));
            }
        }

        /// <summary>
        /// Gets the application UUID.
        /// </summary>
        /// <value>
        /// The UUID.
        /// </value>
        public Guid Uuid
        {
            get
            {
                return new Guid(this.UuidInternal);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            string format = "{0}, version {1}.{2} by {3}";
            return string.Format(format, this.AppName, this.AppMajorVersion, this.AppMinorVersion, this.CompanyName);
        }
    }
}
