using System;
using System.Diagnostics;
using Eggshell.Debugging.Commands;
using Eggshell.Debugging.Logging;

namespace Eggshell
{
	/// <summary>
	/// Eggshell's core Debugging Library. Has support for
	/// logging, commands, overlays, and other utility features.
	/// </summary>
	[Library, Group( "Debug" )]
	public static class Terminal
	{
		/// <summary>
		/// Command Console. Use Run(string, object[]) to run a command.
		/// Your game can have its own Console provider.
		/// </summary>
		public static ICommander Command { get; set; }

		/// <summary>
		/// Add your own extension methods if need be, since this is an
		/// instanced class. 
		/// </summary>
		public static ILogger Log { get; set; }

		/// <summary>
		/// Runs a stopwatch on a IDisposable Scope. Use this in a using() expression
		/// to record how long it took to execute that code block.
		/// </summary>
		public static IDisposable Stopwatch( string message = null, bool alwaysReport = false )
		{
			return ReportStopwatch || alwaysReport ? new TimedScope( message ) : null;
		}

		static Terminal()
		{
			Log = new Logger();
			Command = new Commander();
		}

		// Debug

		[Terminal, Property( "dev.report_stopwatch", true )]
		private static bool ReportStopwatch { get; set; } = true;

		private class TimedScope : IDisposable
		{
			private readonly Stopwatch _stopwatch;
			private readonly string _message;

			public TimedScope( string message )
			{
				_message = message;

				_stopwatch = System.Diagnostics.Stopwatch.StartNew();
			}

			public void Dispose()
			{
				_stopwatch.Stop();

				var time = _stopwatch.Elapsed.Seconds > 0 ? $"{_stopwatch.Elapsed.TotalSeconds} seconds" : $"{_stopwatch.Elapsed.TotalMilliseconds} ms";

				if ( string.IsNullOrEmpty( _message ) )
				{
					Log.Info( time );
					return;
				}

				Log.Info( $"{_message} | {time}" );
			}
		}
	}
}
