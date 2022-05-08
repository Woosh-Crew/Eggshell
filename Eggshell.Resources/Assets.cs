using System.Linq;
using Eggshell.IO;

namespace Eggshell.Resources
{
	public sealed class Assets : Module
	{
		public static Registry Registered { get; } = new();

		public override void OnReady()
		{
			// No valid pathing right now...
			return;
			
			// Setup Resources
			foreach ( var pathing in Library.Database.All<IAsset>().Select( e => e.Components.Get<PathAttribute>() ) )
			{
				foreach ( var file in Files.Pathing( $"{pathing.ShortHand}://" ).All() )
				{
					Registered.Fill( file.Virtual().Normalise() );
				}
			}
		}

		public override void OnShutdown()
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

		private void Sweep()
		{
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
		}
		
		*/

		// API

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

		public static T Fallback<T>() where T : class, IAsset, new()
		{
			Library library = typeof( T );

			// Load default resource, if its not there
			if ( !library.Components.TryGet( out FileAttribute files ) || files.Fallback.IsEmpty() )
			{
				return null;
			}

			Terminal.Log.Error( $"Loading fallback for [{library.Title}]" );

			Pathing fallback = files.Fallback;
			fallback = fallback.Virtual().Normalise();

			return !fallback.Exists() ? null : Load<T>( fallback, true );
		}

		public static Resource Find( Pathing path )
		{
			path = path.Virtual().Normalise();
			return Registered[path] != null ? Registered[path] : (path.Exists() ? Registered.Fill( path ) : null);
		}

		public static Resource Find( int hash )
		{
			return Registered[hash];
		}
	}
}
