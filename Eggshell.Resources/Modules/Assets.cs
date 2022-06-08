using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Eggshell.IO;

namespace Eggshell.Resources
{
	/// <summary>
	/// The assets module is responsible for handling the loading and unloading
	/// of resources and assets. This API will help you greatly when trying to
	/// make data loaded at runtime and compiled at runtime.
	/// </summary>
	public sealed class Assets : Module, IEnumerable<Resource>
	{
		/// <summary>
		/// A reference to all the registered resources, loaded or not. You can get
		/// resources by its path or identifier and load them manually.
		/// </summary>
		public static Assets Registered => Get<Assets>();

		// Resources API
		// --------------------------------------------------------------------------------------- //

		/// <summary>
		/// Manually load an asset from its hash, and if it doesn't exist creat it from
		/// a func that returns a resource. Useful when loading resources straight from memory
		/// or from web requests. It is recommended you just use the Pathing version though.
		/// </summary>
		public static T Load<T>( int hash, Func<Resource> creation, bool persistant = false ) where T : class, IAsset, new()
		{
			return (Find( hash ) ?? Registered.Fill( hash, creation ))?.Load<T>( persistant );
		}

		/// <summary>
		/// Loads an asset from its path. by using its Virtual.Hash as its identifier.
		/// If the asset you are trying to load has a shorthand path, it'll automatically
		/// apply that to the path if its not there.
		/// </summary>
		public static T Load<T>( Pathing path, bool persistant = false ) where T : class, IAsset, new()
		{
			Library library = typeof( T );

			// Apply shorthand, if path doesn't have one
			if ( !path.IsValid() && library.Components.TryGet<PathAttribute>( out var attribute ) )
			{
				path = $"{attribute.ShortHand}://" + path;
			}

			var resource = Find( path.Virtual().Normalise() );
			return resource != null ? resource.Load<T>( persistant ) : Fallback<T>();
		}

		/// <summary>
		/// Loads a fallback assets of the type T, such as with a model itd load
		/// the error model, or a sound, it'll load some funky sound.
		/// </summary>
		public static T Fallback<T>() where T : class, IAsset, new()
		{
			Library library = typeof( T );

			// Load default resource, if its not there
			if ( !library.Components.TryGet( out Archive files ) || files.Fallback.IsEmpty() )
			{
				return null;
			}

			Terminal.Log.Error( $"Loading fallback for [{library.Title}]" );

			Pathing fallback = files.Fallback;
			fallback = fallback.Virtual().Normalise();

			return !fallback.Exists() ? null : Load<T>( fallback, true );
		}

		/// <summary>
		/// Trys to find a resource by its path, if it doesn't exist and its a
		/// valid path, it'll create a resource at that path.
		/// </summary>
		public static Resource Find( Pathing path, bool fill = true )
		{
			path = path.Virtual().Normalise();
			return Registered[path] != null ? Registered[path] : (path.Exists() && fill ? Registered.Fill( path ) : null);
		}

		/// <summary>
		/// Trys to find a resource by its identifier / hash. Returns null
		/// if nothing was found.
		/// </summary>
		public static Resource Find( int hash )
		{
			return Registered[hash];
		}

		// Compiler API
		// --------------------------------------------------------------------------------------- //

		public static void Compile<T>( T item )
		{
			var library = Library.Database.Find( typeof( ICompiler<T> ) );
			Assert.IsNull( library );

			Library.Create<ICompiler<T>>( library.Info )?.Compile( item );
		}

		// Module Logic
		// --------------------------------------------------------------------------------------- //

		protected override void OnReady()
		{
			/*
			foreach ( var pathing in Library.Database.All<IAsset>().Select( e => e.Components.Get<PathAttribute>() ) )
			{
				foreach ( var file in Files.Pathing( $"{pathing.ShortHand}://" ).All() )
				{
					Registered.Fill( file );
				}
			}
			*/
		}

		protected override void OnShutdown()
		{
			foreach ( var resource in Registered )
			{
				resource.Unload( true );
			}
		}

		// Sweep

		/*
		
		private RealTimeSince _timeSinceSweep;
		private const int _timeBetweenSweeps = 60;
		
		*/

		private void Sweep()
		{
			/*
			
			if ( !(_timeSinceSweep > _timeBetweenSweeps) )
			{
				return;
			}

			_timeSinceSweep = 0;

			foreach ( var resource in Registered )
			{
				if ( resource.Instances?.Count == 0 && !resource.Persistant )
				{
					Terminal.Log.Info( $"No Instances of [{resource.Path}], Unloading" );
					resource.Unload( false );
				}
			}
			
			*/
		}

		// Registry
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
		public Resource this[ Pathing key ] => _storage.TryGetValue( key.Virtual().Normalise().Hash(), out var resource ) ? resource : null;

		/// <summary>
		/// Fills a slot on the resources storage by its raw identifier.
		/// It'll return the slot that's currently being used from the hash,
		/// if not it'll make a not slot for that resource and return that.
		/// </summary>
		public Resource Fill( int hash, Func<Resource> creation )
		{
			if ( Registered[hash] != null )
			{
				return Registered[hash];
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
			if ( Registered[path] != null )
			{
				return Registered[path];
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
