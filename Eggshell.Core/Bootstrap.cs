using System;
using System.Reflection;

namespace Eggshell
{
	public class Bootstrap : ILibrary
	{
		public Library ClassInfo { get; }

		public Bootstrap()
		{
			ClassInfo = Library.Register( this );
		}

		public void Boot()
		{
			OnStart();
			OnModules();
			OnBooted();
		}

		public void Update()
		{
			OnUpdate();
		}

		public void Shutdown()
		{
			OnShutdown();
		}

		// Boot Callbacks

		protected virtual void OnStart() { }

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

		protected virtual void OnBooted()
		{
			foreach ( var module in Module.All )
			{
				module.OnReady();
			}
		}

		// Update Callbacks

		protected virtual void OnUpdate()
		{
			foreach ( var module in Module.All )
			{
				module.OnUpdate();
			}
		}

		// Shutdown Callbacks

		protected virtual void OnShutdown()
		{
			foreach ( var module in Module.All )
			{
				module.OnShutdown();
			}
		}
	}
}
