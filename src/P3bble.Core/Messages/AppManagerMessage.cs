using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using P3bble.Constants;
using P3bble.Helper;
using P3bble.Types;

namespace P3bble.Messages
{
    /// <summary>
    /// Represents the result of an application operation
    /// </summary>
    internal enum AppManagerResult
    {
        /// <summary>
        /// The application is available
        /// </summary>
        AppAvailable = 0,

        /// <summary>
        /// The application was removed
        /// </summary>
        AppRemoved = 1,

        /// <summary>
        /// The application was updated
        /// </summary>
        AppUpdated = 2
    }

    /// <summary>
    /// Represents an app action type
    /// </summary>
    internal enum AppManagerAction : byte
    {
        /// <summary>
        /// The list apps action
        /// </summary>
        ListApps = 1,

        /// <summary>
        /// The remove application action
        /// </summary>
        RemoveApp = 2,

        /// <summary>
        /// The add application action
        /// </summary>
        AddApp = 3,
    }

    internal class AppManagerMessage : P3bbleMessage
    {
        private AppManagerAction _action;
        private uint _appId;
        private uint _appIndex;

        public AppManagerMessage()
            : this(AppManagerAction.ListApps)
        {
        }

        public AppManagerMessage(AppManagerAction action)
            : base(Endpoint.AppManager)
        {
            this._action = action;
        }

        public AppManagerMessage(AppManagerAction action, uint appId, uint appIndex)
            : this(action)
        {
            this._appId = appId;
            this._appIndex = appIndex;
        }

        /// <summary>
        /// Gets the installed applications.
        /// </summary>
        /// <value>
        /// The installed applications.
        /// </value>
        public InstalledApplications InstalledApplications { get; private set; }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        internal AppManagerResult Result { get; private set; }

        protected override void AddContentToMessage(List<byte> payload)
        {
            payload.Add((byte)this._action);

            switch (this._action)
            {
                case AppManagerAction.ListApps:
                    // Just the action required
                    break;

                case AppManagerAction.RemoveApp:
                    byte[] appId = BitConverter.GetBytes(this._appId);
                    byte[] appIndex = BitConverter.GetBytes(this._appIndex);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(appId);
                        Array.Reverse(appIndex);
                    }

                    payload.AddRange(appId);
                    payload.AddRange(appIndex);
                    break;
            }
        }

        protected override void GetContentFromMessage(List<byte> payload)
        {
            const int AppInfoSize = 78;

            AppManagerAction messageType = (AppManagerAction)payload[0];
            byte[] data = payload.ToArray();

            switch (messageType)
            {
                case AppManagerAction.ListApps:
                    byte[] numBanks = new byte[4];
                    byte[] numApps = new byte[4];

                    Array.Copy(data, 1, numBanks, 0, 4);
                    Array.Copy(data, 5, numApps, 0, 4);

                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(numBanks);
                        Array.Reverse(numApps);
                    }

                    uint appBanksAvailable = BitConverter.ToUInt32(numBanks, 0);
                    uint appsInstalled = BitConverter.ToUInt32(numApps, 0);

                    this.InstalledApplications = new InstalledApplications(appBanksAvailable);

                    for (int i = 0; i < appsInstalled; i++)
                    {
                        byte[] id = new byte[4];
                        byte[] index = new byte[4];
                        byte[] name = new byte[32];
                        byte[] company = new byte[32];
                        byte[] flags = new byte[4];
                        byte[] version = new byte[2];

                        int offset = 1 + 8 + (i * AppInfoSize);

                        Array.Copy(data, offset, id, 0, 4);
                        Array.Copy(data, offset + 4, index, 0, 4);
                        Array.Copy(data, offset + 8, name, 0, 32);
                        Array.Copy(data, offset + 40, company, 0, 32);
                        Array.Copy(data, offset + 72, flags, 0, 4);
                        Array.Copy(data, offset + 76, version, 0, 2);

                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(id);
                            Array.Reverse(index);
                            Array.Reverse(flags);
                            Array.Reverse(version);
                        }

                        int nameLength = Array.IndexOf(name, (byte)0);
                        int companyLength = Array.IndexOf(company, (byte)0); 

                        this.InstalledApplications.ApplicationsInstalled.Add(
                            new InstalledApplication()
                            {
                                Id = BitConverter.ToUInt32(id, 0),
                                Index = BitConverter.ToUInt32(index, 0),
                                Name = Encoding.UTF8.GetString(name, 0, nameLength),
                                Company = Encoding.UTF8.GetString(company, 0, companyLength),
                                Flags = BitConverter.ToUInt32(flags, 0),
                                Version = BitConverter.ToUInt16(version, 0)
                            });
                    }

                    break;

                case AppManagerAction.RemoveApp:
                    byte[] rawResult = new byte[4];
                    Array.Copy(data, 1, rawResult, 0, 4);

                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(rawResult);
                    }

                    this.Result = (AppManagerResult)BitConverter.ToInt32(rawResult, 0);
                    break;
            }
        }
    }
}
