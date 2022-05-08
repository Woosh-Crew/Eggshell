﻿using System;
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
	[Serializable, Group( "Reflection" )]
	public class Libraries : ILibrary, IEnumerable<Library>
	{
		private readonly SortedList<int, Library> _storage = new();
		public Library ClassInfo => typeof( Libraries );

		// Accessors
		// --------------------------------------------------------------------------------------- //

		/// <summary>
		/// Finds a library by its deterministic Id
		/// </summary>
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
					Terminal.Log.Error( $"Classname ID [{hash}], was not found in Library Database" );
					return null;
				}
			}
		}

		/// <summary>
		/// Finds a library by its name
		/// </summary>
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
					Terminal.Log.Error( $"Classname {key}, was not found in Library Database" );
					return null;
				}
			}
		}

		/// <summary>
		/// Finds a library by its type
		/// </summary>
		public Library this[ Type key ] => _storage.Values.FirstOrDefault( e => e.Info == key );

		/// <summary>
		/// This will find the library that holds this type. Or will
		/// find a type that is implementing an interface from the inputted
		/// type, (as confusing as that sounds). This is very helpful
		/// for finding types easily. As it handles interface logic
		/// </summary>
		public Library Find( Type type )
		{
			return type.IsInterface
				? this.FirstOrDefault( e => e.Info.HasInterface( type ) && !e.Info.IsAbstract )
				: this.FirstOrDefault( e =>
					(type == e.Info || e.Info.IsSubclassOf( type )) && !e.Info.IsAbstract );
		}

		/// <summary>
		/// This will find the library that holds this type with an added
		/// search pattern, where you can cull results based on components
		/// or what ever. Or will find a type that is implementing an interface
		/// from the inputted type, (as confusing as that sounds).
		/// This is very helpful for finding types easily.
		/// As it handles interface logic
		/// </summary>
		public Library Find( Type type, Func<Library, bool> search )
		{
			return type.IsInterface
				? this.FirstOrDefault( e =>
					e.Info.HasInterface( type ) && !e.Info.IsAbstract && search.Invoke( e ) )
				: this.FirstOrDefault( e =>
					(type == e.Info || e.Info.IsSubclassOf( type )) && !e.Info.IsAbstract && search.Invoke( e ) );
		}

		///  <inheritdoc cref="Find{T}()"/>
		public Library Find<T>() where T : class
		{
			return Find( typeof( T ) );
		}

		///  <inheritdoc cref="Find(Type, Func{Library, bool})"/>
		public Library Find<T>( Func<Library, bool> search ) where T : class
		{
			return Find( typeof( T ), search );
		}

		/// <summary>
		/// This will get all libraries where they are a subclass of
		/// the type T, or will get types that implement the inputted
		/// interface. 
		/// </summary>
		public IEnumerable<Library> All<T>() where T : class
		{
			var type = typeof( T );
			return type.IsInterface ? this.Where( e => e.Info.HasInterface<T>() ) : this.Where( e => e.Info.IsSubclassOf( type ) );
		}

		// API

		public void Add( Library item )
		{
			var hashedName = item.Name.Hash();

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
				Add( new Library( type.Name.ToProgrammerCase(), type ) );
				return;
			}

			// If we have meta present, use it
			var attribute = type.GetCustomAttribute<LibraryAttribute>();
			Add( new Library( attribute!.Name, type ) );
		}

		public void Add( Assembly assembly, bool precompiled = true )
		{
			if ( precompiled )
			{
				assembly.GetType( "Eggshell.Generated.Classroom" )?.GetMethod( "Cache", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static )?.Invoke( null, null );
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

			foreach ( var assembly in domain.GetAssemblies() )
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
