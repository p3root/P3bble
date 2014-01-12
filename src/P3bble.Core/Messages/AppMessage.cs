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

        public AppMessage()
            : this(AppMessageAction.ListApps)
        {
        }

        public AppMessage(AppMessageAction action)
            : base(P3bbleEndpoint.AppManager)
        {
            this._action = action;
        }

        public P3bbleInstalledApplications InstalledApplications { get; set; }

        protected override ushort PayloadLength
        {
            get
            {
                switch (this._action)
                {
                    case AppMessageAction.ListApps:
                        return 1;
                }

                return 0;
            }
        }

        protected override void AddContentToMessage(List<byte> payload)
        {
            switch (this._action)
            {
                case AppMessageAction.ListApps:
                    base.AddContentToMessage(payload);
                    payload.Add((byte)this._action);
                    break;
            }
        }

        protected override void GetContentFromMessage(List<byte> payload)
        {
            const int AppInfoSize = 78;

            AppMessageAction messageType = (AppMessageAction)payload[0];
            byte[] data = payload.ToArray();

            ////byte[] payloadArray = payload.ToArray();
            ////if (BitConverter.IsLittleEndian)
            ////{
            ////    Array.Reverse(payloadArray, 1, 4);
            ////}

            ////int timestamp = BitConverter.ToInt32(payloadArray, 1);
            ////Time = timestamp.AsDateTime();

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
                    /*
                        message_id = unpack("!I", data[1:])
                        message_id = int(''.join(map(str, message_id)))
                        return app_install_message[message_id]
                     */
                    break;
            }
        }
    }
}
