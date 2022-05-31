using System;
using System.Collections.Generic;

namespace Eggshell.Debugging
{
	public sealed class Logger : ILogger
	{
		public IReadOnlyCollection<Entry> All => _logs;
		private readonly List<Entry> _logs = new();

		// Logs

		public void Add( Entry entry )
		{
			if ( string.IsNullOrEmpty( entry.Message ) )
			{
				entry.Message = "n/a";
			}

			entry.Time = DateTime.Now;
			Console.WriteLine( $"[{entry.Time.ToShortTimeString()}] [{entry.Level}] {entry.Message}" );
			_logs.Add( entry );
		}

		public void Clear()
		{
			_logs.Clear();
		}
	}
}
