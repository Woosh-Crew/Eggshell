using System;

namespace Eggshell
{
	[AttributeUsage( AttributeTargets.Class )]
	public class SingletonAttribute : Attribute, IComponent<Library>
	{
		public void OnAttached( Library item ) { }
	}
}
