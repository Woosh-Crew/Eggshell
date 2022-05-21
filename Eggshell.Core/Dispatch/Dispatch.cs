using System;
using System.Collections.Generic;
using Eggshell.Dispatching;

namespace Eggshell
{
	/// <summary>
	/// Fire and forget message dispatching. Primarily used for invoking
	/// methods from a string (string based events) can be used across
	/// languages. (C# - C++, vice versa)
	/// </summary>
	public class Dispatch : Module
	{
		// Dispatch API
		// --------------------------------------------------------------------------------------- //

		/// <summary>
		/// The underlying system that controls the dispatcher. Override
		/// this to add your own dispatching logic.
		/// </summary>
		public static IDispatcher Provider { get; set; } = new Dispatcher();

		/// <summary>
		/// Runs a fire and forget dispatch. Incredibly useful for global
		/// events, where you don't need to create boiler plate code to setup 
		/// </summary>
		public static void Run( string name )
		{
			if ( Provider == null || name.IsEmpty() )
			{
				return;
			}

			Provider.Run( name );
		}

		/// <summary>
		/// Runs a dispatch with an array of args. Use this with caution
		/// as it creates an array with the args.
		/// </summary>
		public static void Run( string name, params object[] args )
		{
			if ( Provider == null || name.IsEmpty() )
			{
				return;
			}

			try
			{
				Provider.Run( name, args );
			}
			catch ( Exception e )
			{
				Terminal.Log.Exception( e );
			}
		}

		/// <summary>
		/// Registers an instanced ILibrary to receive dispatch events.
		/// (Make sure to unregister the object when finished, or else
		/// it wont be collected by the GC)
		/// </summary>
		public static void Register<T>( T item ) where T : class, ILibrary
		{
			Assert.IsNull( item );
			Provider.Register( item );
		}

		/// <summary>
		/// Unregisters an instanced ILibrary from the dispatch system.
		/// (Make sure to call this after you're finished with a registered
		/// object, or else the GC wont collect it)
		/// </summary>
		public static void Unregister<T>( T item ) where T : class, ILibrary
		{
			Assert.IsNull( item );
			Provider.Unregister( item );
		}

		// Predefined Dispatches
		// --------------------------------------------------------------------------------------- //

		public override void OnReady()
		{
			Run( "eggshell.ready" );
		}

		public override void OnUpdate()
		{
			Run( "eggshell.update" );
		}

		public override void OnShutdown()
		{
			Run( "eggshell.shutdown" );
		}
	}
}
