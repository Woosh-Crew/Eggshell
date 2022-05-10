using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace Eggshell.Networking
{
	/// <summary>
	/// Messages is responsible for sending messages between eggshell applications
	/// We can use this for invoking commands in the console from the editor process. 
	/// (Launch maps in game from editor)
	/// </summary>
	public class Messages : Module
	{
		public override void OnShutdown()
		{
			if ( !Game.HasExited && Pipe.IsConnected )
			{
				Writer?.WriteLine( "+quit" );
			}

			Writer?.Close();
			Pipe?.Close();
			Game?.Close();
		}

		public static void Send( string data, string appPath = "compiled://<executable>" )
		{
			// Add to Buffer
			if ( Writer == null )
			{
				Threading.Create( "editor_ipc", new( () => Server( appPath ) ) { IsBackground = true } );
			}

			Threading.Running["editor_ipc"].Enqueue( () => Write( data ) );
		}

		private static void Write( string message )
		{
			try
			{
				Writer.WriteLine( message );
				Terminal.Log.Info( $"[SERVER] Writing: {message}" );
			}
			catch ( IOException e )
			{
				// Catch the IOException that is raised if the pipe is broken or disconnected
				Terminal.Log.Error( $"[SERVER] Error: {e.Message}" );
			}
		}

		private static Process Game { get; set; }
		private static StreamWriter Writer { get; set; }
		private static AnonymousPipeServerStream Pipe { get; set; }

		private static void Server( string applicationPath )
		{
			Game = new() { StartInfo = new( Files.Pathing( applicationPath ).Absolute() ) { UseShellExecute = false } };
			Pipe = new( PipeDirection.Out, HandleInheritability.Inheritable );

			Terminal.Log.Info( "[SERVER] Creating Pipe" );
			Terminal.Log.Info( $"[SERVER] Current Transmission Mode: {Pipe.TransmissionMode}." );

			// Pass the client process a handle to the server.
			Game.StartInfo.Arguments += " -connect " + Pipe.GetClientHandleAsString();
			Game.StartInfo.Arguments += " -screen-fullscreen 0 -screen-height 720 -screen-width 1280 -dev -tools";
			Game.Start();

			Pipe.DisposeLocalCopyOfClientHandle();

			try
			{
				// Read Input from Editor
				Writer = new( Pipe );
				Writer.AutoFlush = true;

				Write( $"Connected to {Process.GetCurrentProcess().ProcessName}" );
				Pipe.WaitForPipeDrain();

				do
				{
					Threading.Running["editor_ipc"].Run();
					Tick();
					Thread.Sleep( TimeSpan.FromSeconds( 0.5f ) );
				} while ( !Game.HasExited && Pipe.IsConnected );
			}
			catch ( IOException e )
			{
				// Catch the IOException that is raised if the pipe is broken or disconnected
				Terminal.Log.Error( $"[SERVER] Error: {e.Message}" );
			}

			Game.WaitForExit();
			Game.Close();

			Shutdown();

			Terminal.Log.Info( "[SERVER] Client quit. Server terminating." );
			Threading.Running["editor_ipc"].Close();
		}

		private static void Tick() { }

		private static void Shutdown()
		{
			Writer?.Dispose();
			Pipe?.Dispose();

			Writer = null;
			Pipe = null;

			Game = null;
		}

		// Game [Client]

		internal static void Connect( string handle )
		{
			Threading.Create( "game_ipc", new( () => Client( handle ) ) { IsBackground = true } );
		}

		private static void Receive( string message )
		{
			if ( message.StartsWith( "+" ) )
			{
				// Call on main thread
			}
		}

		private static void Client( string handle )
		{
			Terminal.Log.Info( $"[CLIENT] Connecting to pipe {handle}" );

			// Open Pipe to Server
			using PipeStream game = new AnonymousPipeClientStream( PipeDirection.In, handle );
			using var sr = new StreamReader( game );

			Terminal.Log.Info( $"[CLIENT] Current Transmission Mode: {game.TransmissionMode}." );

			while ( sr.ReadLine() is { } message )
			{
				Terminal.Log.Info( "[CLIENT] Received Message: " + message );
				Receive( message );
			}
		}
	}
}
