using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using P3bble.Core.Helper;
using SharpGIS;

namespace P3bble.Core.Types
{
    /// <summary>
    /// The bundle type
    /// </summary>
    public enum BundleType
    {
        /// <summary>
        /// Application bundle
        /// </summary>
        Application,

        /// <summary>
        /// Firmware bundle
        /// </summary>
        Firmware
    }

    /// <summary>
    /// Represents an app this.Bundle
    /// <remarks>STRUCT_DEFINITION in pebble.py</remarks>
    /// </summary>
    public class P3bbleBundle
    {
        private P3bbleBundleManifest _manifest;
        private string _privateName;
        private UnZipper _bundle;

        /// <summary>
        /// Create a new Pebblethis.Bundle from a .pwb file and parse its metadata.
        /// </summary>
        /// <param name="path">The relative or full path to the file.</param>
        public P3bbleBundle(string path)
        {
            Stream jsonstream;
            Stream binstream;
            this._privateName = path;

            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            if (!file.FileExists(path))
            {
                throw new FileNotFoundException("the file could not be found in the isolated storage");
            }

            this.FullPath = Path.GetFullPath(path);
            this._bundle = new UnZipper(file.OpenFile(path, FileMode.Open));

            if (this._bundle.FileNamesInZip.Contains("manifest.json"))
            {
                jsonstream = this._bundle.GetFileStream("manifest.json");
            }
            else
            {
                throw new ArgumentException("manifest.json not found in archive - not a Pebble this.Bundle.");
            }

            var serializer = new DataContractJsonSerializer(typeof(P3bbleBundleManifest));

            this._manifest = serializer.ReadObject(jsonstream) as P3bbleBundleManifest;
            jsonstream.Close();

            this.HasResources = this._manifest.Resources.Size != 0;

            if (this._manifest.Type == "firmware")
            {
                this.BundleType = BundleType.Firmware;
            }
            else
            {
                this.BundleType = BundleType.Application;
                if (this._bundle.FileNamesInZip.Contains(this._manifest.Application.Filename))
                {
                    binstream = this._bundle.GetFileStream(this._manifest.Application.Filename);
                }
                else
                {
                    string format = "App file {0} not found in archive";
                    throw new ArgumentException(string.Format(format, this._manifest.Application.Filename));
                }

                this.Application = binstream.AsStruct<P3bbleApplicationMetadata>();
                binstream.Close();
            }
        }

        public BundleType BundleType { get; private set; }

        public bool HasResources { get; private set; }

        public string Filename
        {
            get
            {
                return Path.GetFileName(this.FullPath);
            }
        }

        public string FullPath { get; private set; }

        public P3bbleApplicationMetadata Application { get; private set; }

        public static void DeleteFromStorage(P3bbleBundle bundle)
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            if (file.FileExists(bundle._privateName))
            {
                file.DeleteFile(bundle._privateName);
            }
        }

        public static async Task<string> DownloadFileAsync(string url)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var downloadStream = await response.Content.ReadAsStreamAsync();

                Guid fileGuid = Guid.NewGuid();
                IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();

                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileGuid.ToString(), FileMode.CreateNew, file))
                {
                    byte[] buffer = new byte[1024];
                    while (downloadStream.Read(buffer, 0, buffer.Length) > 0)
                    {
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }

                return fileGuid.ToString();
            }
            else
            {
                return null;
            }
        }

        public override string ToString()
        {
            if (this.BundleType == BundleType.Application)
            {
                string format = "{0} containing watch app {1}";
                return string.Format(format, this.Filename, this.Application);
            }
            else
            {
                // This is pretty ugly, but will do for now.
                string format = "{0} containing fw version {1} for hw rev {2}";
                return string.Format(format, this.Filename, this._manifest.Resources.Version, this._manifest.Firmware.HardwareRevision);
            }
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
    }
}
