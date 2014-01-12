using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using P3bble.Core.Constants;
using P3bble.Core.Helper;
using P3bble.Core.Types;

namespace P3bble.Core.Messages
{
    /// <summary>
    /// Represents the result of an application operation
    /// </summary>
    public enum AppManagerResult
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
    internal enum AppMessageAction : byte
    {
        /// <summary>
        /// The list apps action
        /// </summary>
        ListApps = 1,

        /// <summary>
        /// The remove application action
        /// </summary>
        RemoveApp = 2, // by index ("!bII", 2, appid, index)
        // by uuid  ("b", 0x02) + str(uuid_to_remove)

        /// <summary>
        /// The add application action
        /// </summary>
        AddApp = 3,    // ("!bI", 3, index)
    }

    internal class AppMessage : P3bbleMessage
    {
        private AppMessageAction _action;
        private uint _appId;
        private uint _appIndex;

        public AppMessage()
            : this(AppMessageAction.ListApps)
        {
        }

        public AppMessage(AppMessageAction action)
            : base(P3bbleEndpoint.AppManager)
        {
            this._action = action;
        }

        public AppMessage(AppMessageAction action, uint appId, uint appIndex)
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
        public P3bbleInstalledApplications InstalledApplications { get; private set; }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public AppManagerResult Result { get; private set; }

        protected override ushort PayloadLength
        {
            get
            {
                switch (this._action)
                {
                    case AppMessageAction.ListApps:
                        return 1;

                    case AppMessageAction.RemoveApp:
                        return 1 + 8;
                }

                return 0;
            }
        }

        protected override void AddContentToMessage(List<byte> payload)
        {
            base.AddContentToMessage(payload);
            payload.Add((byte)this._action);

            switch (this._action)
            {
                case AppMessageAction.ListApps:
                    // Just the action required
                    break;

                case AppMessageAction.RemoveApp:
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

            AppMessageAction messageType = (AppMessageAction)payload[0];
            byte[] data = payload.ToArray();

            switch (messageType)
            {
                case AppMessageAction.ListApps:
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

                    this.InstalledApplications = new P3bbleInstalledApplications(appBanksAvailable);

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
                            new P3bbleInstalledApplication()
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

                case AppMessageAction.RemoveApp:
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
