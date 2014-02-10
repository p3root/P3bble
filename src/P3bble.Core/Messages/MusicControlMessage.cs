using P3bble.Core.Constants;
using System.Collections.Generic;
using System.Linq;

namespace P3bble.Core.Messages
{
    internal class MusicControlMessage : P3bbleMessage
    {

		public MusicControls Command { get; private set; }
		
		public MusicControlMessage()
            : base(P3bbleEndpoint.MusicControl)
        {
        }

		protected override void GetContentFromMessage(List<byte> payload)
        {
	        if (payload.Any())
		        Command = (MusicControls) payload.First();
        }
    }
}
