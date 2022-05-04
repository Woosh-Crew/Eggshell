using System;
using Eggshell.Components;
using Eggshell.Reflection;

namespace Eggshell
{
	/// <summary>
	/// Reflection Component for storing what group does this class belong too.
	/// Will override the Library.Group value.
	/// </summary>
	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Interface, Inherited = true )]
	public sealed class GroupAttribute : Attribute, IComponent<IMeta>
	{
		private readonly string _group;

		public GroupAttribute( string group )
		{
			_group = group;
		}

		public void OnAttached( IMeta library )
		{
			library.Group = _group;
		}
	}
}
