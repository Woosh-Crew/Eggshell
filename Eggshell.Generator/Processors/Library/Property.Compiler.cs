using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Eggshell.Generator
{
	public class PropertyCompiler : Processor
	{
		private ImmutableHashSet<ITypeSymbol> Queued { get; set; }

		// Processor
		// --------------------------------------------------------------------------------------- //

		public override bool IsProcessable( SyntaxTree tree )
		{
			Queued = tree.GetRoot()
				.DescendantNodesAndSelf()
				.OfType<ClassDeclarationSyntax>()
				.Select( x => Model.GetDeclaredSymbol( x ) )
				.OfType<ITypeSymbol>()
				.ToImmutableHashSet();

			return Queued.Count > 0;
		}

		public override void OnProcess() { }
		public override void OnFinish() { }

		// 

		private void Create( ITypeSymbol symbol ) { }
	}
}
