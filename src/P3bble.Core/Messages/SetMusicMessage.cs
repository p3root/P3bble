using System.Collections.Generic;
using System.Linq;
using System.Text;
using P3bble.Core.Constants;

namespace P3bble.Core.Messages
{
    internal class SetMusicMessage : P3bbleMessage
    {
        private string _artist;
        private string _album;
        private string _track;
        private ushort _length;
       
        public SetMusicMessage(string artist, string album, string track)
            : base(P3bbleEndpoint.MusicControl)
        {
            this._artist = artist;
            this._album = album;
            this._track = track;
        }

        protected override ushort PayloadLength
        {
            get
            {
                return this._length;
            }
        }

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

            this._length = (ushort)data.Length;

            base.AddContentToMessage(payload);

            payload.AddRange(data);
        }

        protected override void GetContentFromMessage(List<byte> payload)
        {
            base.GetContentFromMessage(payload);
        }
    }
}
