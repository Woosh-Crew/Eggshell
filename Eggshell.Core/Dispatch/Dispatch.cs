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
	public static class Dispatch
	{
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
			if ( Provider is null || name.IsEmpty() )
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
			if ( Provider is null || name.IsEmpty() )
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

		internal struct Info
		{
			// Class
			public Type Class { get; internal set; }
			public bool IsStatic { get; internal set; }

			// Delegate
			public delegate object Action( object target, object[] args );

			private Action _callback;

			public object Invoke( object target = null, object[] args = null )
			{
				return _callback?.Invoke( target, args );
			}

			//
			// Builder
			//

			public Info WithCallback( Action callbackEvent )
			{
				_callback = callbackEvent;
				return this;
			}

			public Info FromType( Type type )
			{
				Class = type;
				return this;
			}

			// Group
			public class Group : List<Info> { }
		}
	}
}
