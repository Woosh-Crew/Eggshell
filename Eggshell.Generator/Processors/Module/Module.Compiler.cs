﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Eggshell.Generator
{
	public class ModuleCompiler : Processor
	{
		private ImmutableHashSet<ITypeSymbol> modules { get; set; }
		private List<string> Generated { get; } = new();

		public override bool IsProcessable( SyntaxTree tree )
		{
			var module = Compilation.GetTypeByMetadataName( "Eggshell.IModule" );

			modules = tree.GetRoot()
				.DescendantNodesAndSelf()
				.OfType<ClassDeclarationSyntax>()
				.Select( x => Model.GetDeclaredSymbol( x ) )
				.OfType<ITypeSymbol>()
				.Where( x => x.AllInterfaces.Contains( module ) && !x.IsAbstract )
				.ToImmutableHashSet();

			return modules.Count > 0;
		}

		public override void OnProcess()
		{
			foreach ( var typeSymbol in modules )
			{
				if ( Processed.Contains( typeSymbol.Name ) )
					continue;

				Processed.Add( typeSymbol.Name );
				Generated.Add( Create( typeSymbol ) );
			}
		}

		public override void OnFinish()
		{
			Add( Finalise(), $"{Compilation.AssemblyName}.Modules" );
		}

		private string Create( ITypeSymbol typeSymbol )
		{
			var name = $"{(typeSymbol.ContainingNamespace != null ? $"{typeSymbol.ContainingNamespace}." : string.Empty)}{typeSymbol.Name}";
			return $@"Module.All.Add( new {name}() );";
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
	public static class Modules
	{{
		private static void Cache()
		{{
			{string.Join( "\n\t\t\t", Generated )}
		}}		
	}}
}}";
		}
	}
}
