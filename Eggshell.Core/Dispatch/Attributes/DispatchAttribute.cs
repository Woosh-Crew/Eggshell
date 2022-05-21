using System;
using Eggshell.Reflection;

namespace Eggshell
{
	[AttributeUsage( AttributeTargets.Method, Inherited = true, AllowMultiple = true )]
	public class DispatchAttribute : Attribute, IComponent<Function>
	{
		public string Name { get; }

		public DispatchAttribute( string name )
		{
			Name = name;
		}

		public void OnAttached( Function item )
		{
			Dispatch.Provider.Add( Name, item );
		}
	}
}
