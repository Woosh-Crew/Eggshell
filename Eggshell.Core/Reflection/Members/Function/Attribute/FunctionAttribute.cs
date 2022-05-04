using System;
using System.Reflection;
using Eggshell.Reflection;

namespace Eggshell
{
	/// <summary>
	/// Properties are variables that are changeable by the editor.
	/// </summary>
	[AttributeUsage( AttributeTargets.Method )]
	public sealed class FunctionAttribute : Attribute
	{
		private string Name { get; }

		public FunctionAttribute() { }

		public FunctionAttribute( string name )
		{
			Name = name;
		}

		public Function CreateRecord( MethodInfo info )
		{
			return new( info, Name );
		}
	}
}
