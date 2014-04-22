using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace P3bbleWP8
{
    /// <summary>
    /// Utility class to help compatibility with WP8
    /// </summary>
    internal class MessageBox
    {
        /// <summary>
        /// Represents a user's response to a message box.
        /// </summary>
        public enum WrappedMessageBoxResult
        {
            /// <summary>
            /// Represents No result
            /// </summary>
            None = 0,

            /// <summary>
            /// Represents OK result
            /// </summary>
            OK = 1,

            /// <summary>
            /// Represents Cancel result
            /// </summary>
            Cancel = 2
        }

        /// <summary>
        /// Specifies the buttons to include when you display a message box.
        /// </summary>
        public enum WrappedMessageBoxButton
        {
            /// <summary>
            /// Represents OK Button
            /// </summary>
            OK = 0,

            /// <summary>
            /// Represents Cancel Button
            /// </summary>
            OKCancel = 1,
        }

        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="messageBoxText">The message box text.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="buttons">The buttons.</param>
        /// <returns>A Task containing the button clicked</returns>
        public static async Task<WrappedMessageBoxResult> Show(string messageBoxText, string caption, WrappedMessageBoxButton buttons)
        {
            WrappedMessageBoxResult result = WrappedMessageBoxResult.None;
            MessageDialog md;

            if (string.IsNullOrEmpty(caption))
            {
                md = new MessageDialog(messageBoxText);
            }
            else
            {
                md = new MessageDialog(messageBoxText, caption);
            }

            switch (buttons)
            {
                case WrappedMessageBoxButton.OK:
                    md.Commands.Add(new UICommand("OK", new UICommandInvokedHandler((cmd) => result = WrappedMessageBoxResult.OK)));
                    break;

                case WrappedMessageBoxButton.OKCancel:
                    md.Commands.Add(new UICommand("OK", new UICommandInvokedHandler((cmd) => result = WrappedMessageBoxResult.OK)));
                    md.Commands.Add(new UICommand("Cancel", new UICommandInvokedHandler((cmd) => result = WrappedMessageBoxResult.Cancel)));
                    md.CancelCommandIndex = (uint)md.Commands.Count - 1;
                    break;
            }

            await md.ShowAsync();
            return result;
        }

        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="messageBoxText">The message box text.</param>
        /// <param name="caption">The caption.</param>
        /// <returns>A Task containing the button clicked</returns>
        public static Task<WrappedMessageBoxResult> Show(string messageBoxText, string caption)
        {
            return Show(messageBoxText, caption, WrappedMessageBoxButton.OK);
        }

        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="messageBoxText">The message box text.</param>
        /// <returns>A Task containing the button clicked</returns>
        public static Task<WrappedMessageBoxResult> Show(string messageBoxText)
        {
            return Show(messageBoxText, null, WrappedMessageBoxButton.OK);
        }
    }
}
