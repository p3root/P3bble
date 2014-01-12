using System;
using System.Runtime.InteropServices;

namespace P3bble.Core.Types
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct P3bbleApplicationMetadata
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public readonly string Header;
        [MarshalAs(UnmanagedType.U1)]
        public readonly byte StructMajorVersion;
        [MarshalAs(UnmanagedType.U1)]
        public readonly byte StructMinorVersion;
        [MarshalAs(UnmanagedType.U1)]
        public readonly byte SDKMajorVersion;
        [MarshalAs(UnmanagedType.U1)]
        public readonly byte SDKMinorVersion;
        [MarshalAs(UnmanagedType.U1)]
        public readonly byte AppMajorVersion;
        [MarshalAs(UnmanagedType.U1)]
        public readonly byte AppMinorVersion;
        [MarshalAs(UnmanagedType.U2)]
        public readonly ushort Size;
        [MarshalAs(UnmanagedType.U4)]
        public readonly uint Offset;
        [MarshalAs(UnmanagedType.U4)]
        public readonly uint CRC;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public readonly string AppName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public readonly string CompanyName;
        [MarshalAs(UnmanagedType.U4)]
        public readonly uint IconResourceID;
        [MarshalAs(UnmanagedType.U4)]
        public readonly uint SymbolTableAddress;
        [MarshalAs(UnmanagedType.U4)]
        public readonly uint Flags;
        [MarshalAs(UnmanagedType.U4)]
        public readonly uint RelocationListStart;
        [MarshalAs(UnmanagedType.U4)]
        public readonly uint RelocationListItemCount;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public readonly string UUID;

        public Version AppVersion
        {
            get
            {
                return new Version(string.Format("{0}.{1}", this.AppMajorVersion, this.AppMinorVersion));
            }
        }

        public Version SDKVersion
        {
            get
            {
                return new Version(string.Format("{0}.{1}", this.SDKMajorVersion, this.SDKMinorVersion));
            }
        }

        public Version StructVersion
        {
            get
            {
                return new Version(string.Format("{0}.{1}", this.StructMajorVersion, this.StructMinorVersion));
            }
        }

        public override string ToString()
        {
            string format = "{0}, version {1}.{2} by {3}";
            return string.Format(format, this.AppName, this.AppMajorVersion, this.AppMinorVersion, this.CompanyName);
        }
    }
}
