using System.Collections.Generic;
using System.Linq;

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

		public static List<IModule> All { get; } = new();

		public static T Get<T>() where T : class, IModule
		{
			return All.FirstOrDefault( e => e is T ) as T;
		}

		public static void Create<T>() where T : class, IModule, new()
		{
			var item = new T();
			All.Add( item );
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
