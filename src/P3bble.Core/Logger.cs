using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;
using Windows.Storage;

namespace P3bble
{
    /// <summary>
    /// Logging to track down problems in production apps
    /// </summary>
    internal static class Logger
    {
        /// <summary>
        /// Gets or sets a value indicating whether logging is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if logging is enabled; otherwise, <c>false</c>.
        /// </value>
        internal static bool IsEnabled { get; set; }

        private static string FileName { get; set; }

        /// <summary>
        /// Writes a line to the debug log.
        /// </summary>
        /// <param name="message">The message.</param>
        internal static void WriteLine(string message)
        {
            Debug.WriteLine(message);

            if (string.IsNullOrEmpty(FileName))
            {
                FileName = string.Format("Log-{0:yyyy-MM-dd-HH-mm-ss}.txt", DateTime.Now);
                Debug.WriteLine("Logger initialised; writing to " + FileName);
            }

            lock (FileName)
            {
                if (IsEnabled)
                {
                    using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (StreamWriter sw = new StreamWriter(store.OpenFile(FileName, FileMode.Append, FileAccess.Write)))
                        {
                            sw.Write(message + "\n");
                        }
                    }
                }
                else
                {
                    // This is a HORRIBLE hack!
                    // There is something strange happening in message sequencing that goes
                    // away when logging to a file - so when RELEASE mode with logging OFF,
                    // this is my only alternative for now. I am a bad person.
                    Thread.Sleep(10);
                }
            }
        }

        /// <summary>
        /// Clears up the current log file.
        /// </summary>
        internal static void ClearUp()
        {
            if (!string.IsNullOrEmpty(FileName))
            {
                lock (FileName)
                {
                    using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        store.DeleteFile(FileName);
                    }
                }

                FileName = null;
            }
        }
    }
}
