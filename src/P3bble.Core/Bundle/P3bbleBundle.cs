using P3bble.Core.Helper;
using SharpGIS;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Bundle
{
    public class P3bbleBundle
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct P3bbleApplicationMetadata
        {
            public string AppVersion { get { return String.Format("{0}.{1}", AppMajorVersion, AppMinorVersion); } }
            public string SDKVersion { get { return String.Format("{0}.{1}", SDKMajorVersion, SDKMinorVersion); } }
            public string StructVersion { get { return String.Format("{0}.{1}", StructMajorVersion, StructMinorVersion); } }

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public readonly String header;
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
            public readonly UInt16 Size;
            [MarshalAs(UnmanagedType.U4)]
            public readonly uint Offset;
            [MarshalAs(UnmanagedType.U4)]
            public readonly uint CRC;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public readonly String AppName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public readonly String CompanyName;
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
            public readonly String UUID;

            public override string ToString()
            {
                String format = "{0}, version {1}.{2} by {3}";
                return String.Format(format, AppName, AppMajorVersion, AppMinorVersion, CompanyName);
            }
        }

        public enum BundleTypes
        {
            Application,
            Firmware
        }

        public BundleTypes BundleType { get; private set; }
        public bool HasResources { get; private set; }

        public string Filename { get { return Path.GetFileName(FullPath); } }
        public string FullPath { get; private set; }
        public P3bbleApplicationMetadata Application { get; private set; }

        private string _privateName;

        private UnZipper Bundle;
        private P3bbleBundleManifest Manifest;

        /// <summary>
        /// Create a new PebbleBundle from a .pwb file and parse its metadata.
        /// </summary>
        /// <param name="path">The relative or full path to the file.</param>
        public P3bbleBundle(String path)
        {
            Stream jsonstream;
            Stream binstream;
            _privateName = path;

            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            if (!file.FileExists(path))
                throw new FileNotFoundException("the file could not be found in the isolated storage");

            FullPath = Path.GetFullPath(path);
            Bundle = new UnZipper(file.OpenFile(path, FileMode.Open));

            if (Bundle.FileNamesInZip.Contains("manifest.json"))
            {
                jsonstream = Bundle.GetFileStream("manifest.json");
            }
            else
            {
                throw new ArgumentException("manifest.json not found in archive - not a Pebble bundle.");
            }

            var serializer = new DataContractJsonSerializer(typeof(P3bbleBundleManifest));


            Manifest = serializer.ReadObject(jsonstream) as P3bbleBundleManifest;
            jsonstream.Close();

            HasResources = (Manifest.Resources.Size != 0);

            if (Manifest.Type == "firmware")
            {
                BundleType = BundleTypes.Firmware;
            }
            else
            {
                BundleType = BundleTypes.Application;
                if (Bundle.FileNamesInZip.Contains(Manifest.Application.Filename))
                {
                    binstream = Bundle.GetFileStream(Manifest.Application.Filename);
                }
                else
                {
                    String format = "App file {0} not found in archive";
                    throw new ArgumentException(String.Format(format, Manifest.Application.Filename));
                }

                Application = binstream.AsStruct<P3bbleApplicationMetadata>();
                binstream.Close();
            }
        }

        public static void DeleteFromStorage(P3bbleBundle bundle)
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            if(file.FileExists(bundle._privateName))
                file.DeleteFile(bundle._privateName);
        }

        public static event OpenReadCompletedEventHandler OpenReadCompleted;

        public static string DownloadFileAsync(string url)
        {
            WebClient webClient = new WebClient();
            Guid fileGuid = Guid.NewGuid();
            webClient.OpenReadAsync(new Uri(url));
            webClient.OpenReadCompleted += (sender, e) =>
                {
                    IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();

                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileGuid.ToString(), FileMode.CreateNew, file))
                    {
                        byte[] buffer = new byte[1024];
                        while (e.Result.Read(buffer, 0, buffer.Length) > 0)
                        {
                            stream.Write(buffer, 0, buffer.Length);
                        }
                    }

                    if (OpenReadCompleted != null)
                        OpenReadCompleted(sender, e);
                };

            return fileGuid.ToString();
        }

        private static bool IsSpaceIsAvailable(long spaceReq)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                long spaceAvail = store.AvailableFreeSpace;
                if (spaceReq > spaceAvail)
                {
                    return false;
                }
                return true;
            }
        }

        public override string ToString()
        {
            if (BundleType == BundleTypes.Application)
            {
                String format = "{0} containing watch app {1}";
                return String.Format(format, Filename, Application);
            }
            else
            {
                // This is pretty ugly, but will do for now.
                String format = "{0} containing fw version {1} for hw rev {2}";
                return String.Format(format, Filename, Manifest.Resources.FriendlyVersion, Manifest.Firmware.HardwareRevision);
            }
        }
    }
}
