using System.Collections.Generic;
using System.Linq;
using System.Text;
using P3bble.Constants;
using P3bble.Types;

namespace P3bble.Messages
{
    internal class MusicMessage : P3bbleMessage
    {
        private string _artist;
        private string _album;
        private string _track;

        public MusicMessage()
            : base(Endpoint.MusicControl)
        {
        }

        public MusicMessage(string artist, string album, string track)
            : base(Endpoint.MusicControl)
        {
            this._artist = artist;
            this._album = album;
            this._track = track;

            // Check lengths...
            this.TrimValue(ref this._artist);
            this.TrimValue(ref this._album);
            this.TrimValue(ref this._track);
        }

        /// <summary>
        /// Gets or sets the control action.
        /// </summary>
        /// <value>
        /// The control action.
        /// </value>
        public MusicControlAction ControlAction { get; set; }

        protected override void AddContentToMessage(List<byte> payload)
        {
            // No idea what this does.  Do it anyway.
            byte[] data = { 16 };

            byte[] artist = Encoding.UTF8.GetBytes(this._artist);
            byte[] album = Encoding.UTF8.GetBytes(this._album);
            byte[] track = Encoding.UTF8.GetBytes(this._track);
            byte[] artistlen = { (byte)artist.Length };
            byte[] albumlen = { (byte)album.Length };
            byte[] tracklen = { (byte)track.Length };

            data = data.Concat(artistlen).Concat(artist).ToArray();
            data = data.Concat(albumlen).Concat(album).ToArray();
            data = data.Concat(tracklen).Concat(track).ToArray();

            payload.AddRange(data);
        }

        protected override void GetContentFromMessage(List<byte> payload)
        {
            this.ControlAction = (MusicControlAction)payload[0];
        }

        /// <summary>
        /// Trims a value to the max length we can send.
        /// </summary>
        /// <param name="value">The value.</param>
        private void TrimValue(ref string value)
        {
            if (value != null && value.Length > 30)
            {
                value = value.Substring(0, 27) + "...";
            }
        }
    }
}
