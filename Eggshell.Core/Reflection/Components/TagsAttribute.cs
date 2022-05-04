using System;
using Eggshell.Components;
using Eggshell.Reflection;

namespace Eggshell
{
	/// <summary>
	/// Reflection Component for storing Tags / Aliases.
	/// </summary>
	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method )]
	public sealed class TagsAttribute : Attribute, IComponent<Library>, IComponent<Property>, IComponent<Function>
	{
		public string[] Tags { get; }

		public TagsAttribute( params string[] tags )
		{
			Tags = tags;
		}

		public void OnAttached( Library library ) { }
		public void OnAttached( Property property ) { }
		public void OnAttached( Function item ) { }
	}
}
