using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P3bble.Core.Types;

namespace P3bble.Core
{
    /// <summary>
    /// Defines the Pebble interface
    /// </summary>
    public interface IP3bble
    {
        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <returns>A bool indicating if the connection was successful</returns>
        Task<bool> ConnectAsync();

        /// <summary>
        /// Pings the device.
        /// </summary>
        /// <returns>An async task to wait if required</returns>
        Task PingAsync();

        /// <summary>
        /// Resets the watch.
        /// </summary>
        /// <returns>An async task to wait if required</returns>
        Task ResetAsync();

        /// <summary>
        /// Gets the time.
        /// </summary>
        /// <returns>An async task to wait that will return the current time</returns>
        Task<DateTime> GetTimeAsync();

        /// <summary>
        /// Sets the time on the Pebble.
        /// </summary>
        /// <param name="newTime">The new time.</param>
        /// <returns>An async task to wait</returns>
        Task SetTimeAsync(DateTime newTime);

        /// <summary>
        /// Gets the latest firmware version.
        /// </summary>
        /// <returns>An async task to wait that will result in firmware info</returns>
        Task<P3bbleFirmwareResponse> GetLatestFirmwareVersionAsync();

        /// <summary>
        /// Gets a list of the installed apps.
        /// </summary>
        /// <returns>An async task to wait that will result in a list of apps</returns>
        Task<P3bbleInstalledApplications> GetInstalledAppsAsync();

        /// <summary>
        /// Remove an installed application from the specified app-bank.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        Task RemoveAppAsync(P3bbleInstalledApplication app);

        /// <summary>
        /// Sets the now playing track.
        /// </summary>
        /// <param name="artist">The artist.</param>
        /// <param name="album">The album.</param>
        /// <param name="track">The track.</param>
        /// <returns>An async task to wait</returns>
        Task SetNowPlayingAsync(string artist, string album, string track);

        /// <summary>
        /// Downloads an application or firmware bundle
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>An async task to wait</returns>
        Task<P3bbleBundle> DownloadBundleAsync(string uri);

        /// <summary>
        /// Installs an application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="launchAfterInstall">Whether to launch after install.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        Task InstallApp(P3bbleBundle app, bool launchAfterInstall = true);

        Task SmsNotificationAsync(string sender, string message);
        
        Task FacebookNotificationAsync(string sender, string message);
        
        Task EmailNotificationAsync(string sender, string subject, string body);
        
        Task PhoneCallAsync(string name, string number, byte[] cookie);
        
        Task RingAsync(byte[] cookie);
        
        Task StartCallAsync(byte[] cookie);

        Task EndCallAsync(byte[] cookie);
    }
}
