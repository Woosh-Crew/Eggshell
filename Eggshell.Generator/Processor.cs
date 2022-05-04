﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Eggshell.Generator
{
	public abstract class Processor
	{
		// Utils

		protected Generator Generator { get; private set; }
		protected SemanticModel Model => Generator.Context.Compilation.GetSemanticModel( Scope );
		protected SyntaxTree Scope => Generator.Scope;
		protected Compilation Compilation => Generator.Context.Compilation;
		protected GeneratorExecutionContext Context => Generator.Context;

		internal void Register( Generator generator )
		{
			Generator = generator;
		}

		// Processor
		// --------------------------------------------------------------------------------------- //

		protected HashSet<string> Processed { get; } = new();

		// Required

		public abstract bool IsProcessable( SyntaxTree tree );
		public abstract void OnProcess();
		public abstract void OnFinish();

		// Utils

		protected virtual bool Add( string src, string name = null, string suffix = "Generated" )
		{
			name ??= $"{Path.GetFileNameWithoutExtension( Scope.FilePath )}";
			name += $".{suffix}";

			if ( Processed.Contains( name ) )
			{
				return false;
			}

			Processed.Add( name );
			Context.AddSource( name, SourceText.From( src, Encoding.UTF8 ) );
			return true;
		}
	}
}
