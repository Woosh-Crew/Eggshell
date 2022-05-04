using System;
using System.Collections.Generic;

namespace Eggshell.Debugging.Logging
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
				return;
			}

			entry.Time = DateTime.Now;
			_logs.Add( entry );
		}

		public void Clear()
		{
			_logs.Clear();
		}
	}
}
