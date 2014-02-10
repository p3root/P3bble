using System;
using P3bble.Core.Constants;

namespace P3bble.Core.EventArguments
{
	public class MusicControlEventArgs : EventArgs
	{
		public MusicControls Command { get; set; }
	}
}
