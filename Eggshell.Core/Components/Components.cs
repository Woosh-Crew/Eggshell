using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Eggshell
{
	public class Components<T> : IEnumerable<IComponent<T>> where T : class
	{
		public Components( T item )
		{
			_owner = item;
		}

		// Database

		private readonly T _owner;
		private readonly List<IComponent<T>> _storage = new();

		// Enumerator

		public IEnumerator<IComponent<T>> GetEnumerator()
		{
			// This shouldn't box. _store.GetEnumerator Does. but Enumerable.Empty shouldn't.
			return _storage.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		// Controllers

		public void Add( IComponent<T> item )
		{
			if ( item == null )
			{
				Terminal.Log.Warning( $"Trying to add a component that was null to {_owner}" );
				return;
			}

			if ( !item.CanAttach( _owner ) )
			{
				return;
			}

			// Replace if its a Singleton
			if ( item is IObject lib && lib.ClassInfo.Components.Has<Singleton>() && TryGet( item.GetType(), out var comp ) )
			{
				Terminal.Log.Warning( $"Replacing Component [{lib.ClassInfo.Name}]. Was Singleton" );
				Replace( comp, item );
				return;
			}

			_storage.Add( item );
			item.OnAttached( _owner );
		}

		public bool Contains( IComponent<T> item )
		{
			return _storage.Contains( item );
		}

		public void Remove( IComponent<T> item )
		{
			item.OnDetached();
			_storage.Remove( item );
		}

		public void Clear()
		{
			foreach ( var comp in _storage )
			{
				comp.OnDetached();
			}

			_storage.Clear();
		}

		// Utility

		public TComp Get<TComp>() where TComp : class
		{
			var index = 0;
			while ( index <= _storage.Count - 1 )
			{
				var item = _storage[index];

				if ( item is TComp comp )
				{
					return comp;
				}

				index++;
			}

			return null;
		}

		public IComponent<T> Get( Type type )
		{
			var index = 0;
			while ( index <= _storage.Count - 1 )
			{
				var item = _storage[index];

				if ( item.GetType() == type )
				{
					return item;
				}

				index++;
			}

			return null;
		}

		// Create

		public TComp Create<TComp>() where TComp : class, IComponent<T>, new()
		{
			var newComp = new TComp();
			Add( newComp );
			return newComp;
		}

		// Replace

		public void Replace( IComponent<T> old, IComponent<T> newComp )
		{
			if ( old == null || newComp == null )
			{
				Terminal.Log.Error( $"Components aren't valid" );
				return;
			}

			if ( !Contains( old ) )
			{
				Terminal.Log.Error( $"Components doesnt contain {old}" );
				return;
			}

			Remove( old );
			Add( newComp );
		}

		// Try Get

		public bool TryGet<TComp>( out TComp output ) where TComp : class
		{
			output = Get<TComp>();
			return output != null;
		}

		public bool TryGet( Type type, out IComponent<T> output )
		{
			output = Get( type );
			return output != null;
		}

		// Has

		public bool Has<TComp>()
		{
			return Has( typeof( TComp ) );
		}

		public bool Has( Type type )
		{
			var index = 0;
			while ( index <= _storage.Count - 1 )
			{
				var item = _storage[index];

				if ( item.GetType() == type || item.GetType().IsSubclassOf( type ) )
				{
					return true;
				}

				index++;
			}

			return false;
		}
	}
}
