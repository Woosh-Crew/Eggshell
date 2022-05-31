using System;
using System.Diagnostics;
using System.Linq;
using Eggshell.Debugging;

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
		/// instanced class. Usually controlled by the bootstrap
		/// </summary>
		public static ILogger Log { get; set; }
		
		/// <summary>
		/// Overlays are things rendered over everything that represent
		/// debug information. This is usually null and depends on what the
		/// bootstrap does to initialize it.
		/// </summary>
		public static IOverlays Overlays { get; set; }

		/// <summary>
		/// Returns true if the instance was launched in developer mode,
		/// meaning there was a launch arg of -dev.
		/// </summary>
		public static bool Developer { get; }
		
		/// <summary>
		/// Returns true if the instance was launched in some sort of editor
		/// that can be used to edit assets and compile them. Set by the bootstrap
		/// (Such as launching Eggshell in the Unity Editor)
		/// </summary>
		public static bool Editor { get; set;  }

		/// <summary>
		/// Should we report the stopwatch logs, or any other terminal
		/// specific things.
		/// </summary>
		public static bool Report { get; set; } = true;

		/// <summary>
		/// Runs a stopwatch on a IDisposable Scope. Use this in a using() expression
		/// to record how long it took to execute that code block.
		/// </summary>
		public static IDisposable Stopwatch( string message = null, bool alwaysReport = false )
		{
			return Report || alwaysReport ? new TimedScope( message ) : null;
		}

		static Terminal()
		{
			var args = Environment.GetCommandLineArgs();
			Developer = args.Contains( "-dev" ) || args.Contains( "-debug" ) || args.Contains( "-editor" );

			Log = new Logger();
			Command = new Commander();
		}

		// Debug

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
