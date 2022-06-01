using System;
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
	public sealed class Library : Member<INamedTypeSymbol>
	{
		public Library( ISymbol symbol ) : base( symbol )
		{
			Class = Factory.OnType( (ITypeSymbol)symbol );
		}

		public override string Compile( out string className )
		{
			// Class name doesn't matter
			className = Factory.OnType( Symbol, false ).Replace( '.', '_' ).Replace( '<', '_' ).Replace( '>', '_' );

			return $@"
[CompilerGenerated]
private class {className} : Library
{{
	public {className}( Library parent = null ) : base( ""{Name}"", {Name.Hash()}, typeof( {Factory.OnType( Symbol, fillGenerics : false )} ), parent )
	{{
		Title = ""{Title}"";
		Group = ""{Group}"";
		Help = ""{Help}"";
		
		// Add Components
		{OnComponents()}

		// Add Properties
		{OnProperties()}

		// Add Functions
		{OnFunctions()}
	}}

	// Overrides
	public override IObject Create()
	{{
		{OnCreate()}
	}}

	protected override IObject Construct()
	{{
		{OnConstructor()}
	}}

	protected override bool OnRegister( IObject value )
	{{
		{OnRegister()}
	}}

	// Components
	{string.Join( "\n", Components )}

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
				Properties.AppendLine( new Property( symbol ).Compile( out var name ) );
				builder.AppendLine( $@"Properties.Add( new {name}() );" );
			}

			return builder.ToString();
		}

		private string OnCreate()
		{
			if ( Bindings.Count == 0 )
			{
				return "return Construct();";
			}

			var builder = new StringBuilder( "return " );

			foreach ( var binding in Bindings )
			{
				builder.Append( $"{binding}.OnCreate() ?? " );
			}

			return builder.Append( "Construct();" ).ToString();
		}

		private string OnRegister()
		{
			if ( Bindings.Count == 0 )
			{
				return "return true;";
			}

			var builder = new StringBuilder( "return " );

			foreach ( var binding in Bindings )
			{
				builder.Append( $"{binding}.OnRegister( value ) && " );
			}

			return builder.Append( "true;" ).ToString();
		}

		private string OnConstructor()
		{
			if ( Symbol.IsAbstract )
			{
				return $@"Terminal.Log.Error(""Can't create {Name}, class is abstract""); return null;";
			}

			if ( Symbol.IsStatic )
			{
				return $@"Terminal.Log.Error(""Can't create {Name}, class is static""); return null;";
			}

			if ( Symbol.InstanceConstructors.All( e => e.Parameters.Length > 0 || e.DeclaredAccessibility is Accessibility.Private or Accessibility.Protected ) )
			{
				return $@"Terminal.Log.Error(""Can't create {Name}, class is has no publicly accessible parameterless constructor""); return null;";
			}

			var potential = Symbol.GetAttributes().FirstOrDefault( e => e.AttributeClass!.Name.StartsWith( "Constructor" ) )?.ConstructorArguments[0].Value;
			return potential?.ToString() ?? $"return new {Class}();";
		}

		private string OnComponents()
		{
			IEnumerable<AttributeData> GetAttributes()
			{
				var symbol = Symbol;

				while ( true )
				{
					foreach ( var attribute in symbol.GetAttributes().Where( e => IsValid( e.AttributeClass ) ) )
					{
						yield return attribute;
					}

					if ( symbol.BaseType == null || !symbol.BaseType.AllInterfaces.Any( e => e.Name.StartsWith( "IObject" ) ) )
					{
						break;
					}

					symbol = symbol.BaseType;
				}
			}

			bool IsValid( INamedTypeSymbol symbol )
			{
				// Normal Symbol
				if ( symbol.GetAttributes().Any( e => e.AttributeClass.Name.StartsWith( "Binding" ) ) )
				{
					return true;
				}

				// Weird shit, because roslyn is retarded

				if ( symbol.Name.Contains( "Attribute" ) )
				{
					return false;
				}

				return Generator.Current.Compilation.GetTypeByMetadataName( Factory.OnType( symbol ) ).GetAttributes().Any( e => e.AttributeClass.Name.StartsWith( "Binding" ) );
			}

			var builder = new StringBuilder();

			foreach ( var symbol in GetAttributes() )
			{
				var varName = $"component_{symbol.AttributeClass!.Name}";

				if ( Bindings.Contains( varName ) )
				{
					continue;
				}

				Components.AppendLine( $"private IBinding {varName};" );
				builder.AppendLine( $"{varName} = {OnBinding( symbol )};\nComponents.Add({varName});" );

				Bindings.Add( varName );
			}

			return builder.ToString();
		}

		private string OnBinding( AttributeData attribute )
		{
			var builder = new StringBuilder();

			builder.Append( $"new {Factory.OnType( attribute.AttributeClass ).Replace( "Attribute", "" )}(" );

			for ( var i = 0; i < attribute.ConstructorArguments.Length; i++ )
			{
				var argument = attribute.ConstructorArguments[i];
				var arg = argument.Value;

				// This is aids...
				if ( argument.Type!.Name.Equals( "string", StringComparison.OrdinalIgnoreCase ) )
				{
					arg = $@"""{arg}""";
				}

				builder.Append( arg );

				if ( i != attribute.ConstructorArguments.Length - 1 )
				{
					builder.Append( ", " );
				}
			}

			builder.Append( "){" );

			foreach ( var args in attribute.NamedArguments )
			{
				var arg = args.Value.Value;

				// This is aids...
				if ( args.Value.Type.Name.Equals( "string", StringComparison.OrdinalIgnoreCase ) )
				{
					arg = $@"""{arg}""";
				}

				builder.AppendLine( $"{args.Key} = {arg}," );
			}

			return builder.Append( '}' ).ToString();
		}

		private string OnFunctions()
		{
			var builder = new StringBuilder();

			foreach ( var symbol in Symbol.GetMembers().Where( e => Function.IsValid( e, Symbol ) ) )
			{
				Functions.AppendLine( new Function( symbol ).Compile( out var name ) );
				builder.AppendLine( $@"Functions.Add( new {name}() );" );
			}

			return builder.ToString();
		}

		// Library

		public string Class { get; }

		public List<string> Bindings { get; } = new();
		public StringBuilder Components { get; } = new();
		public StringBuilder Properties { get; } = new();
		public StringBuilder Functions { get; } = new();

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
