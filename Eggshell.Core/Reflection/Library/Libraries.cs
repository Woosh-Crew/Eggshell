using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Eggshell
{
	/// <summary>
	/// Libraries is used for handling every library instance. It is
	/// also responsible for caching libraries too. Its in its own
	/// class so we can decouple it, plus provide a wrapper / our own
	/// accessors to the database.
	/// </summary>
	public class Libraries : IEnumerable<Library>
	{
		private readonly SortedList<int, Library> _storage = new();

		// Accessors
		// --------------------------------------------------------------------------------------- //

		public Library this[ int hash ]
		{
			get
			{
				try
				{
					return _storage[hash];
				}
				catch ( KeyNotFoundException )
				{
					Terminal.Log.Error( $"ClassName ID [{hash}], was not found in Library Database" );
					return null;
				}
			}
		}

		public Library this[ string key ]
		{
			get
			{
				try
				{
					return _storage[key.Hash()];
				}
				catch ( KeyNotFoundException )
				{
					Terminal.Log.Error( $"ClassName {key}, was not found in Library Database" );
					return null;
				}
			}
		}

		public Library this[ Type key ] => _storage.Values.FirstOrDefault( e => e.Info == key );

		public Library Find( Type type )
		{
			return type.IsInterface
				? this.FirstOrDefault( e => e.Info.HasInterface( type ) && !e.Info.IsAbstract )
				: this.FirstOrDefault( e =>
					(type == e.Info || e.Info.IsSubclassOf( type )) && !e.Info.IsAbstract );
		}

		public Library Find( Type type, Func<Library, bool> search )
		{
			return type.IsInterface
				? this.FirstOrDefault( e =>
					e.Info.HasInterface( type ) && !e.Info.IsAbstract && search.Invoke( e ) )
				: this.FirstOrDefault( e =>
					(type == e.Info || e.Info.IsSubclassOf( type )) && !e.Info.IsAbstract && search.Invoke( e ) );
		}

		public Library Find<T>() where T : class
		{
			return this.Find( typeof( T ) );
		}

		public Library Find<T>( Func<Library, bool> search ) where T : class
		{
			return this.Find( typeof( T ), search );
		}

		public IEnumerable<Library> All<T>() where T : class
		{
			var type = typeof( T );
			return type.IsInterface ? this.Where( e => e.Info.HasInterface<T>() ) : this.Where( e => e.Info.IsSubclassOf( type ) );
		}

		// API

		public void Add( Library item )
		{
			var hashedName = item.Name!.Hash();

			// Store it in Database
			if ( _storage.ContainsKey( hashedName ) )
			{
				Terminal.Log.Warning( $"Replacing Library [{item.Name}]" );
				_storage[hashedName] = item;
				return;
			}

			_storage.Add( hashedName, item );
		}

		public void Add( Type type )
		{
			if ( !type.IsDefined( typeof( LibraryAttribute ), false ) )
			{
				Add( new Library( type ) );
				return;
			}

			// If we have meta present, use it
			var attribute = type.GetCustomAttribute<LibraryAttribute>();
			Add( attribute!.CreateRecord( type ) );
		}

		public void Add( Assembly assembly )
		{
			var container = assembly.GetType( "Eggshell.Generated.Classroom" );

			if ( container != null )
			{
				Console.WriteLine( "Using Cache" );
				container.GetMethod( "Cache", BindingFlags.NonPublic | BindingFlags.Static )?.Invoke( null, null );
				return;
			}

			foreach ( var type in assembly.GetTypes() )
			{
				if ( Library.IsValid( type ) )
				{
					Add( type );
				}
			}
		}

		public void Add( AppDomain domain )
		{
			var main = typeof( Library ).Assembly;
			Add( main );

			foreach ( var assembly in AppDomain.CurrentDomain.GetAssemblies().Where( e => e.IsDefined( typeof( LibraryAttribute ) ) ) )
			{
				if ( assembly != main )
				{
					Add( assembly );
				}
			}
		}

		// Enumerator

		public IEnumerator<Library> GetEnumerator()
		{
			return _storage.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
