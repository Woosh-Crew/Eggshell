using System;
using Eggshell.Reflection;

namespace Eggshell
{
	/// <summary>
	/// Reflection Component that changes the Tile value on a Library or Property.
	/// </summary>
	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, Inherited = false )]
	public sealed class TitleAttribute : Attribute
	{
		public TitleAttribute( string title ) { }
	}
}
