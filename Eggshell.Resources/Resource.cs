using System;
using System.Collections.Generic;
using System.IO;
using Eggshell.IO;

namespace Eggshell.Resources
{
	public sealed class Resource : ILibrary
	{
		public Library ClassInfo { get; }

		private Resource()
		{
			ClassInfo = Library.Register( this );
		}

		public Resource( Pathing path ) : this()
		{
			Identifier = path.Virtual().Hash();
			Extension = path.Extension();
			Stream = () => path.Info<FileInfo>().OpenRead();
		}

		public Resource( int hash, string extension, Func<Stream> stream ) : this()
		{
			Identifier = hash;
			Extension = extension;
			Stream = stream;
		}

		public override int GetHashCode()
		{
			return Identifier;
		}

		public override string ToString()
		{
			return $"loaded:[{IsLoaded}]id:[{Identifier}]";
		}

		// State

		public bool Persistant { get; set; }
		public bool IsLoaded => Source != null;

		// References

		public IAsset Source { get; private set; }
		public List<IAsset> Instances { get; private set; }
		public Func<Stream> Stream { get; }

		// Identification

		public int Identifier { get; }
		public string Extension { get; }

		// Management

		public T Create<T>() where T : class, IAsset, new()
		{
			Assert.IsTrue( Source != null );

			Source = new T();
			Source.Resource = this;
			Source.Setup( Extension );

			return Source as T;
		}

		public T Load<T>( bool persistant = false ) where T : class, IAsset, new()
		{
			Persistant ^= persistant;

			Library library = typeof( T );

			if ( !IsLoaded )
			{
				var stopwatch = Terminal.Stopwatch( $"Loaded {library.Title} [{Identifier}]" );

				Instances = new();
				Source = Create<T>();

				using ( var stream = Stream.Invoke() )
				{
					Source.Load( stream );
				}

				stopwatch.Dispose();
			}

			var instance = Source.Clone();

			if ( instance == null || instance == Source )
			{
				return (T)Source;
			}

			Instances.Add( instance );
			instance.Resource = this;

			return instance as T;
		}

		public void Unload( bool forcefully )
		{
			if ( !IsLoaded )
			{
				// Nothing was loaded
				return;
			}

			foreach ( var instance in Instances )
			{
				if ( instance == Source )
				{
					continue;
				}

				instance.Delete();
			}

			Instances.Clear();

			if ( forcefully || !Persistant )
			{
				Terminal.Log.Info( $"Unloading {Source.ClassInfo.Title} [{Identifier}]" );

				Source.Unload();
				Source.Delete();
				Source = null;
			}
		}
	}
}
