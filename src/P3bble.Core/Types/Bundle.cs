using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using P3bble.Helper;
using SharpCompress.Archive;
using SharpCompress.Archive.Zip;
using Windows.Storage;

namespace P3bble.Types
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
    public class Bundle
    {
        private string _path;
        private IArchive _bundle;

        /// <summary>
        /// Create a new Pebblethis.Bundle from a .pwb file and parse its metadata.
        /// </summary>
        /// <param name="path">The relative or full path to the file.</param>
        internal Bundle(string path)
        {
            this._path = path;
        }

        /// <summary>
        /// Gets the type of the bundle.
        /// </summary>
        /// <value>
        /// The type of the bundle.
        /// </value>
        public BundleType BundleType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the bundle has resources.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the bundle has resources.
        /// </value>
        public bool HasResources { get; private set; }

        /// <summary>
        /// Gets the filename.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        public string Filename
        {
            get
            {
                return Path.GetFileName(this.FullPath);
            }
        }

        /// <summary>
        /// Gets the full path.
        /// </summary>
        /// <value>
        /// The full path.
        /// </value>
        public string FullPath { get; private set; }

        /// <summary>
        /// Gets the application details.
        /// </summary>
        /// <value>
        /// The application.
        /// </value>
        public ApplicationMetadata Application { get; private set; }

        internal byte[] BinaryContent { get; private set; }

        internal byte[] Resources { get; private set; }

        internal BundleManifest Manifest { get; private set; }

        /// <summary>
        /// Loads a bundle from ApplicationData.Current.LocalFolder.
        /// </summary>
        /// <param name="name">The name of the file in ApplicationData.Current.LocalFolder.</param>
        /// <returns>A bundle</returns>
        public static async Task<Bundle> LoadFromLocalStorageAsync(string name)
        {
            var bundle = new Bundle(name);
            await bundle.Initialise();
            return bundle;
        }

        /// <summary>
        /// Deletes the bundle from storage.
        /// </summary>
        /// <returns>An async task to await</returns>
        public async Task DeleteFromStorage()
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(this._path);
            if (file != null)
            {
                await file.DeleteAsync();
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
            if (this.BundleType == BundleType.Application)
            {
                string format = "{0} containing watch app {1}";
                return string.Format(format, this.Filename, this.Application);
            }
            else
            {
                // This is pretty ugly, but will do for now.
                string format = "{0} containing fw version {1} for hw rev {2}";
                return string.Format(format, this.Filename, this.Manifest.Resources.Version, this.Manifest.Firmware.HardwareRevision);
            }
        }

        internal async Task Initialise()
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(this._path);
            if (file == null)
            {
                throw new FileNotFoundException("the file could not be found in the isolated storage");
            }

            this.FullPath = file.Path;
            this._bundle = ZipArchive.Open(await file.OpenStreamForReadAsync());
           
            var manifestEntry = this._bundle.Entries.Where(e => string.Compare(e.Key, "manifest.json", StringComparison.OrdinalIgnoreCase) == 0).FirstOrDefault();
            if (manifestEntry == null)
            {
                throw new ArgumentException("manifest.json not found in archive - not a Pebble this.Bundle.");
            }

            using (Stream jsonstream = manifestEntry.OpenEntryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(BundleManifest));
                this.Manifest = serializer.ReadObject(jsonstream) as BundleManifest;
            }

            if (this.Manifest.Type == "firmware")
            {
                this.BundleType = BundleType.Firmware;
                this.BinaryContent = await this.ReadFileToArray(this.Manifest.Firmware.Filename, this.Manifest.Firmware.Size);
            }
            else
            {
                this.BundleType = BundleType.Application;
                this.BinaryContent = await this.ReadFileToArray(this.Manifest.ApplicationManifest.Filename, this.Manifest.ApplicationManifest.Size);

                // Convert first part to app manifest
#if NETFX_CORE  && !WINDOWS_PHONE_APP
                byte[] buffer = new byte[Marshal.SizeOf<ApplicationMetadata>()];
#else
                byte[] buffer = new byte[Marshal.SizeOf(typeof(ApplicationMetadata))];
#endif
                Array.Copy(this.BinaryContent, 0, buffer, 0, buffer.Length);
                this.Application = buffer.AsStruct<ApplicationMetadata>();
            }

            this.HasResources = this.Manifest.Resources.Size != 0;
            if (this.HasResources)
            {
                this.Resources = await this.ReadFileToArray(this.Manifest.Resources.Filename, this.Manifest.Resources.Size);
            }
        }

        private async Task<byte[]> ReadFileToArray(string file, int size)
        {
            var entry = this._bundle.Entries.Where(e => string.Compare(e.Key, file, StringComparison.OrdinalIgnoreCase) == 0).FirstOrDefault();

            if (entry == null)
            {
                string format = "App file {0} not found in archive";
                throw new ArgumentException(string.Format(format, file));
            }

            using (Stream stream = entry.OpenEntryStream())
            {
                byte[] result = new byte[size];
                await stream.ReadAsync(result, 0, result.Length);
                return result;
            }
        }
    }
}
