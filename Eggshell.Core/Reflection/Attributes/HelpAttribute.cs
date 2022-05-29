using System;

namespace Eggshell
{
	/// <summary>
	/// Reflection Component that changes the Tile value on a Library or Property.
	/// </summary>
	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, Inherited = false )]
	public sealed class HelpAttribute : Attribute
	{
		public HelpAttribute( string help ) { }
	}
}
