using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Eggshell.Reflection
{
	public class Members<T, TInfo> : IEnumerable<T> where T : class, IMember<TInfo> where TInfo : MemberInfo
	{
		internal static readonly Dictionary<Type, HashSet<T>> Registry = new();

		public Members( Library owner )
		{
			_owner = owner;
		}

		// IDatabase

		public int Count => _storage.Count;

		public T this[ int hash ] => _storage[hash];

		public T this[ string key ]
		{
			get
			{
				var entry = key.Hash();
				return _storage.ContainsKey( entry ) ? _storage[entry] : null;
			}
		}

		// Instance

		private readonly SortedList<int, T> _storage = new();
		private readonly Library _owner;

		// Enumerator

		public IEnumerator<T> GetEnumerator()
		{
			// This shouldn't box. _store.GetEnumerator Does. but Enumerable.Empty shouldn't.
			return Count == 0 ? Enumerable.Empty<T>().GetEnumerator() : _storage.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		// API

		public void Add( T item )
		{
			// Add the Key to the registry, if its null
			if ( !Registry.ContainsKey( item.Info.DeclaringType! ) )
			{
				Registry.Add( item.Info.DeclaringType, new() );
			}

			// See if we can add it to the registry, so it prevents duplicate members
			if ( Registry[item.Info.DeclaringType].All( e => e.Name != item.Name ) )
			{
				Registry[item.Info.DeclaringType].Add( item );
			}

			// Assign Proper owner, if we're the owner.
			if ( item.Owner == null && (item.IsStatic || item.Info.DeclaringType == _owner.Info) )
			{
				item.Owner = _owner;
				item.Group = item.Group.IsEmpty( _owner.Title );
			}

			if ( item.Identifier == default )
			{
				item.Identifier = item.Name.Hash();
			}

			// Now add it to the instance
			if ( _storage.ContainsKey( item.Identifier ) )
			{
				Terminal.Log.Error( $"Replacing {item.Name}, from {_owner.Name}" );
				return;
			}

			_storage.Add( item.Identifier, item );
		}
	}
}
