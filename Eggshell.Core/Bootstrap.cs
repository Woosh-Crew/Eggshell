using System;
using System.Reflection;

namespace Eggshell
{
	/// <summary>
	/// Bootstrap is responsible for initializing all eggshell modules for your
	/// application as well as responsible for controlling them. This is overrideable
	/// so you can use this in existing applications / frameworks / engines incredibly easy.
	/// </summary>
	public class Bootstrap : IObject
	{
		public Library ClassInfo { get; }

		/// <summary> 
		/// Creates a new instance of the bootstrap class. This should 
		/// be only constructed one  and used in the Project.Crack to
		/// initialize your application.
		/// </summary>
		public Bootstrap()
		{
			ClassInfo = Library.Register( this );

			if ( ClassInfo == null )
			{
				Terminal.Log.Error( "Failed to register Bootstrap." );
			}
		}

		internal void Boot()
		{
			OnStart();
			OnModules();
			OnBooted();
		}

		/// <summary>
		/// Invoke Update, to run the update loop on all modules, you
		/// can override this by overriding the protected method OnUpdate;
		/// </summary>
		public void Update()
		{
			OnUpdate();
		}

		/// <summary>
		/// Invoke Shutdown, to run the shutdown call chain on all modules, you
		/// can override this by overriding the protected method OnShutdown;
		/// </summary>
		public void Shutdown()
		{
			OnShutdown();
		}

		// Boot Callbacks

		/// <summary>
		/// OnStart is called by Boot() before any of the modules have been
		/// initialized. Use this for doing pre-initialization.
		/// </summary>
		protected virtual void OnStart() { }

		/// <summary>
		/// Override OnModules to change how modules are cached and registered.
		/// You should never need to override this, but just in case it is.
		/// </summary>
		protected virtual void OnModules()
		{
			// Cache Modules using Reflection
			foreach ( var assembly in AppDomain.CurrentDomain.GetAssemblies() )
			{
				assembly.GetType( "Eggshell.Generated.Modules" )?
					.GetMethod( "Cache", BindingFlags.Static | BindingFlags.NonPublic )?
					.Invoke( null, null );
			}
		}

		/// <summary>
		/// Called when eggshell is ready, and the bootstrap has done what it
		/// needs to do. This should call OnReady on all modules, so they can
		/// start there systems.
		/// </summary>
		protected virtual void OnBooted()
		{
			foreach ( var module in Module.All )
			{
				module.OnReady();
			}
		}

		// Update Callbacks

		/// <summary>
		/// The update loop for your application, by default this will call
		/// the OnUpdate method on modules. Shouldn't need to override this.
		/// </summary>
		protected virtual void OnUpdate()
		{
			foreach ( var module in Module.All )
			{
				module.OnUpdate();
			}
		}

		// Shutdown Callbacks

		/// <summary>
		/// Called when the application is quitting / shutting down. Use this
		/// to dispose any bootstrap specific resources. Should call OnShutdown
		/// on all modules. 
		/// </summary>
		protected virtual void OnShutdown()
		{
			foreach ( var module in Module.All )
			{
				module.OnShutdown();
			}
		}
	}
}
