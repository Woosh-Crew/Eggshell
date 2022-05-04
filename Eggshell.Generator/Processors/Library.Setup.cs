﻿using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Eggshell.Generator
{
	public class LibrarySetup : Processor
	{
		private ImmutableHashSet<ITypeSymbol> ILibrary { get; set; }

		public override bool IsProcessable( SyntaxTree tree )
		{
			var libraryInterface = Compilation.GetTypeByMetadataName( "Eggshell.ILibrary" );

			ILibrary = tree.GetRoot()
				.DescendantNodesAndSelf()
				.OfType<ClassDeclarationSyntax>()
				.Select( x => Model.GetDeclaredSymbol( x ) )
				.OfType<ITypeSymbol>()
				.Where( x => x.Interfaces.Contains( libraryInterface ) && !x.GetMembers().Any( e => e.Name == "ClassInfo" ) )
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
				Add( Compile( typeSymbol ) );
			}
		}

		public override void OnFinish() { }

		public string Compile( ITypeSymbol typeSymbol )
		{
			// Create the class

			var src = $@"
partial class {typeSymbol.Name} : ILibrary
{{
	private Library _classInfo;
	public Library ClassInfo => _classInfo ??= Library.Register( this );
}}
";
			// Add in the Namespace

			if ( !typeSymbol.ContainingNamespace.IsGlobalNamespace)
			{
				src = $@"
namespace {typeSymbol.ContainingNamespace}
{{
	{src}
}}
";
			}

			// Now Finish it off, by adding the Using
			src = $@"// This class was <autogenerated> by Eggshell.
using Eggshell;

{src}
";
			return src;
		}
	}
}
