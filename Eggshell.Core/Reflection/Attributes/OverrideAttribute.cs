using System;

namespace Eggshell.Reflection
{
	/// <summary>
	/// Overrides something in the source generator, it depends on the generator
	/// and the process its in. Will not work on most generators
	/// </summary>
	public class OverrideAttribute : Attribute
	{
		public OverrideAttribute( string replace ) { }
		public OverrideAttribute( string replace, string with ) { }
	}
}
