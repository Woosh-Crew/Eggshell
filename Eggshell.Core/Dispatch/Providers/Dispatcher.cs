using System.Collections.Generic;
using Eggshell.Reflection;

namespace Eggshell.Dispatching
{
	public class Dispatcher : IDispatcher
	{
		public Library ClassInfo { get; }

		public Dispatcher()
		{
			ClassInfo = Library.Register( this );
		}

		private Dictionary<string, List<Function>> _events = new();
		private Dictionary<Library, List<ILibrary>> _registry = new();

		public void Add( string eventName, Function function )
		{
			if ( !_events.ContainsKey( eventName ) )
			{
				_events.Add( eventName, new() );
			}

			_events[eventName]?.Add( function );
		}

		public void Run( string name )
		{
			Run( name, null );
		}

		public void Run( string name, params object[] args )
		{
			if ( !_events.TryGetValue( name, out var callbacks ) )
			{
				return;
			}

			foreach ( var function in callbacks )
			{
				if ( function.IsStatic )
				{
					function.Invoke( null, args );
					continue;
				}

				foreach ( var item in _registry[function.Parent] )
				{
					function.Invoke( item, args );
				}
			}
		}

		public void Register( ILibrary item )
		{
			var type = item.GetType();

			if ( !_registry.ContainsKey( type ) )
			{
				_registry.Add( type, new() );
			}

			if ( _registry.TryGetValue( type, out var all ) )
			{
				all.Add( item );
			}
		}

		public void Unregister( ILibrary item )
		{
			if ( _registry.TryGetValue( item.GetType(), out var all ) )
			{
				all.Remove( item );
			}
		}

		public void Dispose()
		{
			_registry?.Clear();
			_registry = null;

			_events?.Clear();
			_events = null;
		}
	}
}
