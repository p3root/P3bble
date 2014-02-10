using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Constants
{
	public enum MusicControls
	{
		PlayPause = 1,
		Forward = 4,
		Previous = 5,
		// PlayPause also sends 8 for some reason.  To be figured out.
		Other = 8
	}
}
