using System;
using Eggshell.Reflection;

namespace Eggshell
{
	/// <summary>
	/// Reflection Component for storing what group does this class belong too.
	/// Will override the Library.Group value.
	/// </summary>
	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Interface )]
	public sealed class GroupAttribute : Attribute
	{
		public GroupAttribute( string group ) { }
	}
}
