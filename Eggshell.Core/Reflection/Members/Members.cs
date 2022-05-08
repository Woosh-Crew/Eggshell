﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Eggshell.Reflection
{
	public class Members<T, TInfo> : IEnumerable<T> where TInfo : MemberInfo where T : class, IMember<TInfo>
	{
		private readonly SortedList<int, T> _storage = new();
		public Library Parent { get; }

		public Members( Library parent )
		{
			Parent = parent;
		}

		public void Add( T item )
		{
			item.Parent ??= Parent;

			if ( _storage.ContainsKey( item.Id ) )
			{
				Terminal.Log.Error( $"Replacing {item.Name}, from {Parent.Name}" );
				_storage[item.Id] = item;
				return;
			}

			_storage.Add( item.Id, item );
		}

		internal void Add( Members<T, TInfo> inherited )
		{
			foreach ( var prop in inherited._storage.Values )
			{
				Add( prop );
			}
		}

		// Enumerator

		public IEnumerator<T> GetEnumerator()
		{
			return _storage.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		// Accessors
		// --------------------------------------------------------------------------------------- //

		/// <summary>
		/// Finds a member by its deterministic Id
		/// </summary>
		public T this[ int hash ]
		{
			get
			{
				try
				{
					return _storage[hash];
				}
				catch ( KeyNotFoundException )
				{
					Terminal.Log.Error( $"Classname ID [{hash}], was not found in Library Database" );
					return null;
				}
			}
		}

		/// <summary>
		/// Finds a member by its name
		/// </summary>
		public T this[ string key ]
		{
			get
			{
				try
				{
					return _storage[key.Hash()];
				}
				catch ( KeyNotFoundException )
				{
					Terminal.Log.Error( $"Classname {key}, was not found in Library Database" );
					return null;
				}
			}
		}
	}
}
