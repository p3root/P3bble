using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P3bble.Types;

namespace P3bble
{
    /// <summary>
    /// Defines the Pebble interface
    /// </summary>
    public interface IPebble
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
        Task<FirmwareResponse> GetLatestFirmwareVersionAsync();

        /// <summary>
        /// Gets a list of the installed apps.
        /// </summary>
        /// <returns>An async task to wait that will result in a list of apps</returns>
        Task<InstalledApplications> GetInstalledAppsAsync();

        /// <summary>
        /// Remove an installed application from the specified app-bank.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        Task<bool> RemoveAppAsync(InstalledApplication app);

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
        Task<Bundle> DownloadBundleAsync(string uri);

        /// <summary>
        /// Installs an application.
        /// </summary>
        /// <param name="bundle">The application.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        Task InstallApp(Bundle bundle);

        /// <summary>
        /// Launches an application.
        /// </summary>
        /// <param name="appUuid">The application UUID.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        Task<bool> LaunchApp(Guid appUuid);

        /// <summary>
        /// Installs a firmware bundle.
        /// </summary>
        /// <param name="bundle">The firmware.</param>
        /// <param name="recovery">Whether to install recovery firmware.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        Task InstallFirmware(Bundle bundle, bool recovery);

        /// <summary>
        /// Sends an SMS notification.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        /// <remarks>Mainly for demoing capability</remarks>
        Task SmsNotificationAsync(string sender, string message);

        /// <summary>
        /// Sends a Facebook notification.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        /// <remarks>Mainly for demoing capability</remarks>
        Task FacebookNotificationAsync(string sender, string message);

        /// <summary>
        /// Sends an Email notification.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        /// <remarks>
        /// Mainly for demoing capability
        /// </remarks>
        Task EmailNotificationAsync(string sender, string subject, string body);

        /// <summary>
        /// Starts a Phone call notification.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="number">The number.</param>
        /// <param name="cookie">The cookie.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        /// <remarks>
        /// Mainly for demoing capability
        /// </remarks>
        Task PhoneCallAsync(string name, string number, byte[] cookie);

        /// <summary>
        /// Starts a Phone call Ring.
        /// </summary>
        /// <param name="cookie">The cookie.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        /// <remarks>
        /// Mainly for demoing capability
        /// </remarks>
        Task RingAsync(byte[] cookie);

        /// <summary>
        /// Indicate that a Phone call has started.
        /// </summary>
        /// <param name="cookie">The cookie.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        /// <remarks>
        /// Mainly for demoing capability
        /// </remarks>
        Task StartCallAsync(byte[] cookie);

        /// <summary>
        /// Indicate that a Phone call has ended.
        /// </summary>
        /// <param name="cookie">The cookie.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        /// <remarks>
        /// Mainly for demoing capability
        /// </remarks>
        Task EndCallAsync(byte[] cookie);
    }
}
