using System;

namespace Eggshell.Generator
{
	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Method )]
	public class ExportAttribute : Attribute
	{
		// Exports a Class or Method, for use in another language
	}
}
