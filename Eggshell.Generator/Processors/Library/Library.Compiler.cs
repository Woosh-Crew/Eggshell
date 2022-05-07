﻿using System;
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
			{
				Create( typeSymbol );
			}
		}

		public override void OnFinish()
		{
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
			if ( hasBaseType )
			{
				Create( typeSymbol.BaseType );
			}

			var output = $@"
var {name.Replace( '.', '_' )} = new Library( ""{GetName( typeSymbol )}"", typeof( {name} ), {(hasBaseType ? ($"{(typeSymbol.BaseType.ContainingNamespace != null ? $"{typeSymbol.BaseType.ContainingNamespace}." : string.Empty)}{typeSymbol.BaseType.Name}").Replace( '.', '_' ) : "null")} )
{{
	Title = ""{GetTitle( typeSymbol )}"",
	Group = ""{GetGroup( typeSymbol )}"",
	Help = @""{GetHelp( typeSymbol )}"",
}};
";

			Generated.Add( output );
			Names.Add( name.Replace( '.', '_' ) );
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
			var syntax = typeSymbol.DeclaringSyntaxReferences.First().GetSyntax().GetLeadingTrivia().Where( e => e.IsKind( SyntaxKind.SingleLineCommentTrivia ) ).Select( e => e.ToFullString() ).ToArray();

			if ( syntax.Length == 0 )
			{
				return "n/a";
			}

			// This is so fucking aids... dont change it
			return string.Join( "", syntax ).Replace( "<summary>", "" ).Replace( "</summary>", "" ).Replace( "///", "" ).Replace( "\"", "\"\"" ).Trim().Replace( "\n", " " );
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
			var addEverything = new StringBuilder();

			foreach ( var name in Names )
			{
				addEverything.AppendLine( $"Library.Database.Add({name});" );
			}

			return $@"// This was generated by Eggshell.
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
			{addEverything.ToString()}
		}}		
	}}
}}";
		}
	}
}
