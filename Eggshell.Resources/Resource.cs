using System;
using System.Collections.Generic;
using System.IO;
using Eggshell.IO;

namespace Eggshell.Resources
{
	/// <summary>
	/// A resource holds a reference and its state to an
	/// asset. It uses a stream for loading the asset.
	/// </summary>
	public sealed class Resource : ILibrary
	{
		public Library ClassInfo { get; }

		private Resource()
		{
			ClassInfo = Library.Register( this );
			Components = new( this );
		}

		internal Resource( Pathing path ) : this( path.Virtual().Hash(), path.Name( false ), path.Extension(), () => path.Info<FileInfo>().OpenRead() )
		{
			Components.Create<Origin>().Path = path;
		}

		internal Resource( int hash, string name, string extension, Func<Stream> stream ) : this()
		{
			Name = name;
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

		// Identification

		public string Name { get; }
		public int Identifier { get; }
		public string Extension { get; }

		// State

		public bool Persistant { get; private set; }
		public bool IsLoaded => Source != null;

		// References

		public IAsset Source { get; private set; }
		public List<IAsset> Instances { get; private set; }
		public Func<Stream> Stream { get; }
		public Components<Resource> Components { get; }

		// Management

		private T Create<T>() where T : class, IAsset, new()
		{
			Assert.IsTrue( Source != null );

			Source = new T();
			Source.Resource = this;
			Source.Setup( Extension );

			return Source as T;
		}

		private void Load()
		{
			using var stopwatch = Terminal.Stopwatch( $"Loaded Resource [{Name}, {Identifier}]" );
			using var stream = Stream.Invoke();
			Source.Load( stream );
		}

		public T Load<T>( bool persistant = false ) where T : class, IAsset, new()
		{
			Persistant ^= persistant;

			if ( !IsLoaded )
			{
				Instances = new();
				Source = Create<T>();
				Load();
			}

			return Clone<T>();
		}

		private T Clone<T>() where T : class, IAsset, new()
		{
			var instance = Source.Clone();

			if ( instance == null || instance == Source )
			{
				return (T)Source;
			}

			Instances.Add( instance );
			instance.Resource = this;

			return (T)instance;
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
				Terminal.Log.Info( $"Unloading Resource [{Name}, {Identifier}]" );

				Source.Unload();
				Source.Delete();

				Source = null;
			}
		}

		public void Delete()
		{
			Assert.IsTrue( IsLoaded, "Can't delete a loaded resource" );
			Assets.Registered.Remove( this );
		}
	}
}
