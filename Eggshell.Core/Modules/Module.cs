using System.Collections.Generic;

namespace Eggshell
{
	/// <summary>
	/// Module is Eggshells object oriented application injection. Modules will
	/// be automatically created when you initialize Eggshell
	/// </summary>
	[Singleton]
	public abstract class Module : IModule
	{
		// Static API
		// --------------------------------------------------------------------------------------- //
		
		public static IEnumerable<IModule> All => _all;

		private static IModule _cached;
		private static readonly List<IModule> _all = new();

		/// <summary>
		/// Try's to get a module by its type and will return it if it could
		/// find it in the registry. Will return null if it couldn't find it.
		/// </summary>
		public static T Get<T>() where T : class, IModule
		{
			if ( _cached is T cached )
			{
				return cached;
			}

			for ( var i = 0; i < _all.Count; i++ )
			{
				if ( _all[i] is not T comp )
					continue;

				_cached = comp;
				return comp;
			}

			return null;
		}

		/// <summary>
		/// Try's to create a module by its type and will add it to the registered
		/// list if it successes in doing so. Calls to this function will automatically
		/// be created by source generators, and invoked when the egg is cracked.
		/// </summary>
		public static void Create<T>() where T : class, IModule, new()
		{
			var item = new T();

			if ( item.ClassInfo == null )
			{
				Terminal.Log.Warning( "ClassInfo for Module was null" );
				return;
			}


			if ( !item.OnRegister() )
			{
				Terminal.Log.Warning( $"{item.ClassInfo.Title} couldn't be registered" );
				return;
			}

			_all.Add( item );
		}

		// Instance
		// --------------------------------------------------------------------------------------- //

		public Library ClassInfo { get; }

		public Module()
		{
			ClassInfo = Library.Register( this );
		}

		public virtual bool OnRegister() { return true; }

		public virtual void OnReady() { }
		public virtual void OnUpdate() { }
		public virtual void OnShutdown() { }
	}
}
