using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Eggshell.Generator
{
	public class LibraryCompiler : Processor
	{
		private ImmutableHashSet<ITypeSymbol> ILibrary { get; set; }
		private List<string> Generated { get; } = new();

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
			{
				if ( Processed.Contains( typeSymbol.Name ) )
					continue;

				Processed.Add( typeSymbol.Name );
				Generated.Add( Create( typeSymbol ) );
			}
		}

		public override void OnFinish()
		{
			Add( Finalise(), $"{Compilation.AssemblyName}.Classroom" );
		}

		private string Create( ITypeSymbol typeSymbol )
		{
			var name = $"{(typeSymbol.ContainingNamespace != null ? $"{typeSymbol.ContainingNamespace}." : string.Empty)}{typeSymbol.Name}";

			return $@"
Library.Database.Add( new Library( typeof( {name} ), ""{GetName( typeSymbol )}"" )
{{
	
	Title = ""{GetTitle( typeSymbol )}"",
	Group = ""{GetGroup( typeSymbol )}"",
	Help = @""{GetHelp( typeSymbol )}"",
}} );
";
		}

		private string GetName( ITypeSymbol symbol )
		{
			var attribute = symbol.GetAttributes().FirstOrDefault( e => e.AttributeClass!.Name.StartsWith( "Library" ) );

			if ( attribute == null || attribute.ConstructorArguments.Length == 0 )
				return ToProgrammerCase( symbol.Name, symbol.ContainingNamespace?.ToString() );

			return (string)attribute.ConstructorArguments[0].Value;
		}

		private string GetHelp( ITypeSymbol typeSymbol )
		{
			var summary = typeSymbol.DeclaringSyntaxReferences
				.First()
				.GetSyntax()
				.GetLeadingTrivia()
				.FirstOrDefault( e => e.IsKind( SyntaxKind.SingleLineDocumentationCommentTrivia ) )
				.ToFullString().Split( new[] { "///" }, StringSplitOptions.RemoveEmptyEntries );

			return summary.Length == 0 ? string.Empty : Between( string.Join( " ", summary ), " <summary>", " </summary>" ).Replace( "\"", "\"\"" ).Replace( "\n", " " );

		}

		private string Between( string input, string first, string last )
		{
			var pos1 = input.IndexOf( first, StringComparison.OrdinalIgnoreCase ) + first.Length;
			var pos2 = input.IndexOf( last, StringComparison.OrdinalIgnoreCase );
			return input.Substring( pos1, pos2 - pos1 );
		}

		private string GetTitle( ITypeSymbol typeSymbol )
		{
			var title = (string)typeSymbol.GetAttributes().FirstOrDefault( e => e.AttributeClass!.Name.StartsWith( "Title" ) )?.ConstructorArguments[0].Value;
			title ??= CultureInfo.CurrentCulture.TextInfo.ToTitleCase( string.Concat( typeSymbol.Name.Select( x => char.IsUpper( x ) ? " " + x : x.ToString() ) ).TrimStart( ' ' ) );
			return title;
		}

		private string GetGroup( ITypeSymbol typeSymbol )
		{
			var group = (string)typeSymbol.GetAttributes().FirstOrDefault( e => e.AttributeClass!.Name.StartsWith( "Group" ) )?.ConstructorArguments[0].Value;
			group ??= typeSymbol.ContainingNamespace.IsGlobalNamespace ? "" : typeSymbol.ContainingNamespace.Name;
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
			return $@"
// This classroom, was created by Eggshell.
using Eggshell;
using System.Runtime.CompilerServices;

namespace Eggshell.Generated
{{
	[CompilerGenerated]
	public static class Classroom
	{{
		public static void Cache()
		{{
			{string.Join( "\n\t\t\t", Generated )}
		}}		
	}}
}}";
		}
	}
}
