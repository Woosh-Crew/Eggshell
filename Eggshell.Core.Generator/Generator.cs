using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Eggshell.Generator
{
	[Generator]
	public class Generator : ISourceGenerator
	{
		public List<Exception> Exceptions { get; } = new();
		public List<Processor> Processors { get; } = new();

		public void Register( params Processor[] processors )
		{
			foreach ( var processor in processors )
			{
				processor.Register( this );
				Processors.Add( processor );
			}
		}

		public void Register<T>() where T : Processor, new()
		{
			var newT = new T();
			newT.Register( this );
			Processors.Add( newT );
		}

		public void Register( Type type )
		{
			var newT = (Processor)Activator.CreateInstance( type );

			if ( newT == null )
			{
				return;
			}

			newT.Register( this );
			Processors.Add( newT );
		}

		// Main Generator
		// --------------------------------------------------------------------------------------- //

		public virtual void Initialize( GeneratorInitializationContext context )
		{
			// Init Library Generators
			//	Register<LibrarySetup>(); /* This is disabled, because you I want it to be init in a constructor */  
			Register<LibraryCompiler>();
			Register<ModuleCompiler>();
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
						if ( process.IsProcessable( syntaxTree ) )
						{
							process.OnProcess();
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
