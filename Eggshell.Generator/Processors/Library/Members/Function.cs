using System.Linq;
using Microsoft.CodeAnalysis;

namespace Eggshell.Generator
{
	/// <summary>
	/// Creates a function from a Symbol, that is used in Eggshells
	/// compile time reflection system.
	/// </summary>
	public class Function : Member<IMethodSymbol>
	{
		public Function( ISymbol symbol ) : base( symbol )
		{
			Owner = Factory.OnType( symbol.ContainingType );
		}

		public override string Compile( out string className )
		{
			className = $"{Owner}.{Symbol.Name}".Replace( '.', '_' );

			return $@"
[CompilerGenerated]
private class {className} : Function
{{
	public {className}() : base( ""{Name}"", ""{Symbol.Name}"" )
	{{
		Title = ""{Title}"";
		Group = ""{Group}"";
		Help = @""{Help}"";
		IsStatic = {(Symbol.IsStatic ? "true" : "false")};
	}}
}}
";
		}

		protected override string OnName( ISymbol symbol )
		{
			var attribute = symbol.GetAttributes().FirstOrDefault( e => e.AttributeClass!.Name.StartsWith( "Function" ) );

			if ( attribute is { ConstructorArguments.Length: > 0 } )
				return (string)attribute.ConstructorArguments[0].Value;

			return base.OnName( symbol );
		}

		// Property

		public string Owner { get; }

		// Static

		public static bool IsValid( ISymbol symbol, ITypeSymbol typeSymbol )
		{
			return symbol.Kind == SymbolKind.Method
			       && !symbol.IsOverride
			       && !symbol.Name.StartsWith( ".ctor" )
			       && !symbol.Name.StartsWith( "op_" )
			       && !symbol.Name.StartsWith( "get_" )
			       && !symbol.Name.StartsWith( "set_" )
			       && symbol.ContainingType.Equals( typeSymbol, SymbolEqualityComparer.Default )
			       && symbol.GetAttributes().Any( e => e.AttributeClass!.AllInterfaces.Any( e => e.Name.StartsWith( "IComponent" ) ) ) ||
			       symbol.GetAttributes().Any( attribute => attribute.AttributeClass!.Name.StartsWith( "Function" ) );
		}
	}
}
