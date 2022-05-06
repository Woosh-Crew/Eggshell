using System.Collections.Generic;

namespace Eggshell
{
	/// <summary>
	/// Module is Eggshells object oriented
	/// application injection. Modules will
	/// be automatically created when you
	/// initialize Eggshell
	/// </summary>
	public abstract class Module : IModule
	{
		public static List<IModule> All { get; } = new();
		public Library ClassInfo { get; }

		public Module()
		{
			ClassInfo = Library.Register( this );
		}

		public virtual bool OnRegister() { return true; }
		public virtual void OnReady() { }
		public virtual void OnShutdown() { }
	}
}
