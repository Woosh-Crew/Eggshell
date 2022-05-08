using System;

namespace Eggshell.Debugging.Logging
{
	public struct Entry
	{
		public DateTime Time { get; set; }
		public string Message { get; set; }
		public string Trace { get; set; }
		public string Level { get; set; }
	}
}
