using System.Collections;
using System.Collections.Generic;
using Eggshell.IO;

namespace Eggshell.Resources
{
	public class Registry : IEnumerable<Resource>
	{
		private readonly SortedList<int, Resource> _storage = new();

		public Resource this[ int key ] => _storage.ContainsKey( key ) ? _storage[key] : null;

		public Resource this[ Pathing key ]
		{
			get
			{
				var hash = key.Hash();
				return _storage.ContainsKey( hash ) ? _storage[hash] : null;
			}
		}

		// Enumerator

		public IEnumerator<Resource> GetEnumerator()
		{
			return _storage.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		// API

		public Resource Fill( Pathing path )
		{
			var hash = path.Virtual().Hash();

			if ( Assets.Registered[hash] != null )
			{
				return Assets.Registered[hash];
			}

			var instance = new Resource( path );

			_storage.Add( instance.Identifier, instance );
			return instance;
		}
	}
}
