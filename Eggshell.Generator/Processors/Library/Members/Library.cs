using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Eggshell.Generator
{
	/// <summary>
	/// Creates a library from a Symbol, that is used in Eggshells
	/// compile time reflection system.
	/// </summary>
	public sealed class Library : Member<ITypeSymbol>
	{
		public Library( ISymbol symbol ) : base( symbol )
		{
			Class = Factory.OnType( (ITypeSymbol)symbol );
		}

		public override string Compile( out string className )
		{
			// Class name doesn't matter
			className = Class.Replace( '.', '_' );

			return $@"
[CompilerGenerated]
private class {className} : Library
{{
	public {className}( Library parent = null ) : base( ""{Name}"", typeof( {Class} ), parent )
	{{
		Title = ""{Title}"";
		Group = ""{Group}"";
		Help = ""{Help}"";
		
		// Add Components

		// Add Properties
		{OnProperties()}

		// Add Functions
		{OnFunctions()}
	}}

	// Functions
	{string.Join( "\n", Functions )}

	// Properties
	{string.Join( "\n", Properties )}
}}
";
		}

		private string OnProperties()
		{
			var builder = new StringBuilder();

			foreach ( var symbol in Symbol.GetMembers().Where( e => Property.IsValid( e, Symbol ) ) )
			{
				Properties.Add( new Property( symbol ).Compile( out var name ) );
				builder.AppendLine( $@"Properties.Add( new {name}() );" );
			}

			return builder.ToString();
		}

		private string OnFunctions()
		{
			var builder = new StringBuilder();

			foreach ( var symbol in Symbol.GetMembers().Where( e => Function.IsValid( e, Symbol ) ) )
			{
				Functions.Add( new Function( symbol ).Compile( out var name ) );
				builder.AppendLine( $@"Functions.Add( new {name}() );" );
			}

			return builder.ToString();
		}

		// Library

		public string Class { get; }

		public List<string> Properties { get; } = new();
		public List<string> Functions { get; } = new();

		protected override string OnName( ISymbol symbol )
		{
			var attribute = symbol.GetAttributes().FirstOrDefault( e => e.AttributeClass!.Name.StartsWith( "Library" ) );

			if ( attribute is { ConstructorArguments.Length: > 0 } )
				return (string)attribute.ConstructorArguments[0].Value;

			return base.OnName( symbol );
		}

		protected override string OnGroup( ISymbol symbol )
		{
			var attribute = symbol.GetAttributes().FirstOrDefault( e => e.AttributeClass!.Name.StartsWith( "Group" ) );

			if ( attribute is { ConstructorArguments.Length: > 0 } )
				return (string)attribute.ConstructorArguments[0].Value;

			return symbol.ContainingNamespace.Name;
		}
	}
}
