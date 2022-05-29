using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Eggshell.Generator
{
	[Generator]
	public class Generator : ISourceGenerator
	{
		public List<Exception> Exceptions { get; } = new();

		public Processor[] Processors { get; } = new Processor[]
		{
			new BinderCompiler(),
			new LibraryCompiler(),
			new ModuleCompiler()
		};

		public static Processor Current { get; private set; }

		// Main Generator
		// --------------------------------------------------------------------------------------- //

		public virtual void Initialize( GeneratorInitializationContext context )
		{
			foreach ( var processor in Processors )
			{
				processor.Register( this );
			}
		}

		public void Execute( GeneratorExecutionContext context )
		{
			Context = context;
			var compilation = context.Compilation;

			foreach ( var syntaxTree in compilation.SyntaxTrees )
			{
				Scope = syntaxTree;

				foreach ( var process in Processors )
				{
					try
					{
						// This is awesome
						if ( process.IsProcessable( syntaxTree ) )
						{
							Current = process;
							Current.OnProcess();
						}
					}
					catch ( Exception e )
					{
						Exceptions.Add( e );
					}
				}
			}

			foreach ( var process in Processors )
			{
				try
				{
					process.OnFinish();
				}
				catch ( Exception e )
				{
					Exceptions.Add( e );
				}
			}

			foreach ( var exception in Exceptions )
			{
				context.ReportDiagnostic( Diagnostic.Create(
					"EGG001",
					"Source Generation",
					"Generation Exception: " + exception,
					DiagnosticSeverity.Warning,
					DiagnosticSeverity.Warning,
					true, 5, description : exception.StackTrace ) );
			}
		}

		// Eggshell Generator Wrappers
		// --------------------------------------------------------------------------------------- //

		public GeneratorExecutionContext Context { get; set; }
		public SyntaxTree Scope { get; set; }
	}
}
