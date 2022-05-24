using System;

namespace Eggshell
{
	/// <summary>
	/// Attribute that allows the definition of a custom constructor.
	/// Must return an ILibrary and Must have one parameter that takes in a Library.
	/// </summary>
	[AttributeUsage( AttributeTargets.Class )]
	public sealed class ConstructorAttribute : Attribute
	{
		public ConstructorAttribute( string constructor ) { }
	}
}
