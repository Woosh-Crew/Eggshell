using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Eggshell.Generator
{
	public class LibraryCompiler : Processor
	{
		private ImmutableHashSet<ITypeSymbol> ILibrary { get; set; }
		private List<string> Generated { get; } = new();
		private List<string> Names { get; } = new();

		public override bool IsProcessable( SyntaxTree tree )
		{
			var libraryInterface = Compilation.GetTypeByMetadataName( "Eggshell.ILibrary" );

			ILibrary = tree.GetRoot()
				.DescendantNodesAndSelf()
				.OfType<ClassDeclarationSyntax>()
				.Select( x => ModelExtensions.GetDeclaredSymbol( Model, x ) )
				.OfType<ITypeSymbol>()
				.Where( x => x.AllInterfaces.Contains( libraryInterface ) )
				.ToImmutableHashSet();

			return ILibrary.Count > 0;
		}

		public override void OnProcess()
		{
			foreach ( var typeSymbol in ILibrary )
				Create( typeSymbol );
		}

		public override void OnFinish()
		{
			if ( Generated.Count == 0 )
			{
				return;
			}

			Add( Finalise(), $"{Compilation.AssemblyName}.Classroom" );
		}

		private void Create( ITypeSymbol typeSymbol )
		{
			var name = $"{(typeSymbol.ContainingNamespace != null ? $"{typeSymbol.ContainingNamespace}." : string.Empty)}{typeSymbol.Name}";

			if ( Processed.Contains( name ) )
				return;

			Processed.Add( name );

			// Item has base type, cache it first.
			var hasBaseType = typeSymbol.BaseType != null && typeSymbol.BaseType.AllInterfaces.Any( e => e.Name.StartsWith( "ILibrary" ) );

			var baseTypeInAssembly = typeSymbol.BaseType?.ContainingAssembly.Equals( Context.Compilation.Assembly, SymbolEqualityComparer.Default ) ?? false;

			if ( hasBaseType && baseTypeInAssembly )
				Create( typeSymbol.BaseType );

			// Dont touch it, its so aids
			var baseTypeName = $"{(typeSymbol.BaseType.ContainingNamespace != null ? $"{typeSymbol.BaseType.ContainingNamespace}." : string.Empty)}{typeSymbol.BaseType.Name}";
			var baseTypeText = hasBaseType && baseTypeInAssembly
				? baseTypeName.Replace( '.', '_' )
				: (baseTypeInAssembly ? "null" : typeSymbol.BaseType.AllInterfaces.Any( e => e.Name.StartsWith( "ILibrary" ) ) ? $"typeof({baseTypeName})" : "null	");

			// Start creating string
			var variableName = name.Replace( '.', '_' );

			var output = $@"
var {variableName}_type =  typeof( {name} );
var {variableName} = new Library( ""{GetName( typeSymbol )}"", {variableName}_type , {baseTypeText} )
{{
	Title = ""{GetTitle( typeSymbol )}"",
	Group = ""{GetGroup( typeSymbol )}"",
	Help = @""{GetHelp( typeSymbol )}"",
}};

{CacheProperties( typeSymbol, variableName, $"{variableName}_type" )}
{CacheFunctions( typeSymbol, variableName, $"{variableName}_type" )}
";

			Generated.Add( output );
			Names.Add( name.Replace( '.', '_' ) );
		}

		private string CacheProperties( ITypeSymbol typeSymbol, string variable, string typeVariable )
		{
			var builder = new StringBuilder();

			foreach ( var symbol in typeSymbol.GetMembers().Where( e => IsValidProperty( e, typeSymbol ) ) )
			{
				builder.AppendLine( $@"{variable}.Properties.Add( 
new Property(""{GetName( symbol )}"", {typeVariable}.GetProperty( ""{symbol.Name}"", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static )  ) 
{{
	Title = ""{GetTitle( symbol )}"",
	Group = ""{GetGroup( symbol )}"",
	Help = @""{GetHelp( symbol )}"",
}}
);" );
			}

			return builder.ToString();
		}

		private string CacheFunctions( ITypeSymbol typeSymbol, string variable, string typeVariable )
		{
			var builder = new StringBuilder();

			foreach ( var symbol in typeSymbol.GetMembers().Where( e => IsValidFunction( e, typeSymbol ) ) )
			{
				builder.AppendLine( $@"{variable}.Functions.Add( 
new Function(""{GetName( symbol )}"", {typeVariable}.GetMethod( ""{symbol.Name}"", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static )  ) 
{{
	Title = ""{GetTitle( symbol )}"",
	Group = ""{GetGroup( symbol )}"",
	Help = @""{GetHelp( symbol )}"",
}}
);" );
			}

			return builder.ToString();
		}

		private bool IsValidProperty( ISymbol symbol, ITypeSymbol typeSymbol )
		{
			return symbol.Kind == SymbolKind.Property
			       && !symbol.Name.StartsWith( "this" )
			       && symbol.ContainingType.Equals( typeSymbol, SymbolEqualityComparer.Default )
			       && (symbol.DeclaredAccessibility == Accessibility.Public || symbol.GetAttributes().Any( attribute =>
			       {
				       var name = attribute.AttributeClass!.Name;
				       return name.StartsWith( "Property" );
			       } ));
		}

		private bool IsValidFunction( ISymbol symbol, ITypeSymbol typeSymbol )
		{
			return symbol.Kind == SymbolKind.Method
			       && symbol.ContainingType.Equals( typeSymbol, SymbolEqualityComparer.Default )
			       && symbol.GetAttributes().Any( attribute =>
			       {
				       var name = attribute.AttributeClass!.Name;
				       return name.StartsWith( "Function" );
			       } );
		}

		private string GetName( ISymbol symbol )
		{
			var attribute = symbol.GetAttributes().FirstOrDefault( e => e.AttributeClass!.Name.StartsWith( "Library" ) );
			if ( attribute != null && attribute.ConstructorArguments.Length > 0 )
			{
				return (string)attribute.ConstructorArguments[0].Value;
			}

			// Class Symbol
			if ( symbol is ITypeSymbol )
			{
				return ToProgrammerCase( symbol.Name, symbol.ContainingNamespace?.ToString() );
			}

			return ToProgrammerCase( symbol.Name, symbol.ContainingType?.ToString() );
		}

		private string GetHelp( ISymbol typeSymbol )
		{
			var syntax = typeSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax().GetLeadingTrivia().Where( e => e.IsKind( SyntaxKind.SingleLineCommentTrivia ) ).Select( e => e.ToFullString() ).ToArray();

			if ( syntax == null || syntax.Length == 0 )
				return "n/a";

			// This is so fucking aids... dont change it
			return string.Join( "", syntax ).Replace( "<summary>", "" ).Replace( "</summary>", "" ).Replace( "///", "" ).Replace( "\"", "\"\"" ).Trim().Replace( "\n", " " );
		}

		private string GetTitle( ISymbol typeSymbol )
		{
			var title = (string)typeSymbol.GetAttributes().FirstOrDefault( e => e.AttributeClass!.Name.StartsWith( "Title" ) )?.ConstructorArguments[0].Value;
			title ??= CultureInfo.CurrentCulture.TextInfo.ToTitleCase( string.Concat( typeSymbol.Name.Select( x => char.IsUpper( x ) ? " " + x : x.ToString() ) ).TrimStart( ' ' ) );
			return title;
		}

		private string GetGroup( ISymbol symbol )
		{
			var group = (string)symbol.GetAttributes().FirstOrDefault( e => e.AttributeClass!.Name.StartsWith( "Group" ) )?.ConstructorArguments[0].Value;
			group ??= (symbol is ITypeSymbol ? (symbol.ContainingNamespace.IsGlobalNamespace ? string.Empty : symbol.ContainingNamespace.Name) : symbol.ContainingType.Name);
			return group;
		}

		private static string ToProgrammerCase( string value, string prefix = null )
		{
			var name = string.Concat( value.Select( x => char.IsUpper( x ) ? "_" + x : x.ToString() ) ).TrimStart( '_' );

			if ( string.IsNullOrEmpty( prefix ) )
				return name.ToLower();

			name = name.Replace( ' ', '_' );

			var range = prefix.Split( '.' );
			return $"{range[range.Length - 1]}.{name}".ToLower();
		}

		private string Finalise()
		{
			var addEverything = new StringBuilder();

			foreach ( var name in Names )
				addEverything.AppendLine( $"Library.Database.Add({name});" );

			return $@"// This was generated by Eggshell.
using Eggshell;
using Eggshell.Reflection;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Eggshell.Generated
{{
	[CompilerGenerated]
	public static class Classroom
	{{
		public static void Cache()
		{{
			{string.Join( "\n\t\t\t", Generated )}
			{addEverything.ToString()}
		}}		
	}}
}}";
		}
	}
}
