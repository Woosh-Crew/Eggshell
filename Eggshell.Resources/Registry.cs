using System;
using System.Collections;
using System.Collections.Generic;
using Eggshell.IO;

namespace Eggshell.Resources
{
	/// <summary>
	/// The registry is responsible for containing references to all loaded 
	/// and unloaded resources. We use this for keeping track of resource state.
	/// </summary>
	public class Registry : IEnumerable<Resource>
	{
		// Public API
		// --------------------------------------------------------------------------------------- //

		/// <summary>
		/// Gets to get a resource by its hash / identifier. This is useful
		/// for loading resources over the network.
		/// </summary>
		public Resource this[ int key ] => _storage.TryGetValue( key, out var resource ) ? resource : null;

		/// <summary>
		/// Gets a resource by its path. Make sure to call virtual before you
		/// try and get the path, or else it'll most likely return the wrong path.
		/// </summary>
		public Resource this[ Pathing key ] => _storage.TryGetValue( key.Hash(), out var resource ) ? resource : null;

		/// <summary>
		/// Fills a slot on the resources storage by its raw identifier.
		/// It'll return the slot that's currently being used from the hash,
		/// if not it'll make a not slot for that resource and return that.
		/// </summary>
		public Resource Fill( int hash, Func<Resource> creation )
		{
			if ( Assets.Registered[hash] != null )
			{
				return Assets.Registered[hash];
			}

			var instance = creation.Invoke();

			_storage.Add( instance.Identifier, instance );
			return instance;
		}

		/// <summary>
		/// Fills a slot on the resources storage by its path.
		/// It'll return the slot that's currently being used from the hash,
		/// if not it'll make a not slot for that resource and return that.
		/// </summary>
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


		// Internal Logic
		// --------------------------------------------------------------------------------------- //

		private readonly SortedList<int, Resource> _storage = new();

		internal void Remove( Resource resource )
		{
			_storage.Remove( resource.Identifier );
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
	}
}
