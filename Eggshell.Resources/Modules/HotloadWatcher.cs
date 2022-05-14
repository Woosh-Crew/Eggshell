using System.IO;

namespace Eggshell.Resources
{
	public class HotloadWatcher : Module
	{
		public FileSystemWatcher Eyes { get; private set; }

		public override bool OnRegister()
		{
			// Only create if we're a developer
			return Terminal.Developer;
		}

		public override void OnReady()
		{
			Eyes = new();

			Eyes.Filter = "*.*";
			Eyes.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
			Eyes.EnableRaisingEvents = true;

			Eyes.Changed += OnChanged;
			Eyes.Created += OnCreated;
			Eyes.Deleted += OnDeleted;
			Eyes.Error += OnError;
		}

		private void OnChanged( object source, FileSystemEventArgs args )
		{
			if ( args.ChangeType != WatcherChangeTypes.Changed )
			{
				return;
			}

			// Location Change?
		}

		private void OnCreated( object source, FileSystemEventArgs args )
		{
			// Fill the resource
			Assets.Find( args.FullPath );
		}

		private void OnDeleted( object source, FileSystemEventArgs args )
		{
			// Was deleted, piss it off
			
			var resource = Assets.Find( args.FullPath );
			if ( resource == null )
			{
				return;
			}

			resource.Unload( true );
			resource.Delete();
		}

		private void OnError( object source, ErrorEventArgs args )
		{
			// Oh no!
			Terminal.Log.Exception( args.GetException() );
		}

		public override void OnShutdown()
		{
			Eyes.Changed -= OnChanged;
			Eyes.Dispose();
		}
	}
}
