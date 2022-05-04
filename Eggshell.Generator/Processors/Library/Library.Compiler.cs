using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using Microsoft.CodeAnalysis;
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
				.Select( x => Model.GetDeclaredSymbol( x ) )
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
				{
					continue;
				}

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
			var variableName = $"{typeSymbol.ContainingNamespace.ToString().Replace( '.', '_' )}_{typeSymbol.Name}";

			return $@"
var {variableName} = new Library( typeof( {typeSymbol.ContainingNamespace}.{typeSymbol.Name} ) )
{{
	
	Title = ""{GetTitle( typeSymbol )}"",
	Group = ""{GetGroup( typeSymbol )}"",
	Help = ""{GetHelp( typeSymbol )}"",
}};
Library.Database.Add( {variableName} );
";
		}

		private string GetHelp( ITypeSymbol typeSymbol )
		{
			return "Very Helpful help";
		}

		private string GetTitle( ITypeSymbol typeSymbol )
		{
			var title = (string)typeSymbol.GetAttributes().FirstOrDefault( e => e.AttributeClass!.Name.StartsWith( "Title" ) )?.ConstructorArguments[0].Value;
			title ??= typeSymbol.Name;

			return title;
		}

		private string GetGroup( ITypeSymbol typeSymbol )
		{
			var group = (string)typeSymbol.GetAttributes().FirstOrDefault( e => e.AttributeClass!.Name.StartsWith( "Group" ) )?.ConstructorArguments[0].Value;
			group ??= typeSymbol.ContainingNamespace.IsGlobalNamespace ? "" : typeSymbol.ContainingNamespace.Name;
			return group;
		}

		private string Finalise()
		{
			return $@"
// This classroom, was created by Eggshell.
using Eggshell;

namespace Eggshell.Generated
{{
	internal static class Classroom
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
