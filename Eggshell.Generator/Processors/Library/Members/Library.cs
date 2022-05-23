﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
			className = Class.Replace( '.', '_' );

			return $@"
[CompilerGenerated]
private class {className} : Library
{{
	public {className}( Library parent = null ) : base( ""{Name}"", {Name.Hash()} ,typeof( {Class} ), parent )
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
	public override ILibrary Create()
	{{
		{OnCreate()}
	}}

	protected override ILibrary Construct()
	{{
		{OnConstructor()}
	}}

	protected override bool OnRegister( ILibrary value )
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

			if ( Symbol.InstanceConstructors.Length > 0 && Symbol.InstanceConstructors.Any( e => e.Parameters.Length > 0 ) )
			{
				return $@"Terminal.Log.Error(""Can't create {Name}, class is has no parameterless constructor""); return null;";
			}

			return $"return new {Class}();";
		}

		private string OnComponents()
		{
			IEnumerable<AttributeData> GetAttributes()
			{
				var symbol = Symbol;

				while ( true )
				{
					foreach ( var attribute in symbol.GetAttributes().Where( e => e.AttributeClass!.AllInterfaces.Any( e => e.Name.StartsWith( "IBinding" ) ) ) )
					{
						yield return attribute;
					}

					if ( symbol.BaseType != null && symbol.BaseType.AllInterfaces.Any( e => e.Name.StartsWith( "ILibrary" ) ) )
					{
						symbol = symbol.BaseType;
					}
					else
					{
						break;
					}
				}
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

			foreach ( var argument in attribute.ConstructorArguments )
			{
				builder.Append( argument.Value );
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