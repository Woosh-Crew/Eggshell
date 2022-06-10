using System;
using System.IO;

namespace Eggshell.Resources
{
	[Group( "Data" )]
	public class Data : IAsset
	{
		public Library ClassInfo { get; }
		public string Name { get; set; }

		public Data()
		{
			ClassInfo = Library.Register( this );
		}

		// Compile

		internal void Compile( BinaryWriter writer )
		{
			writer.Write( ClassInfo.Id );

			// This is automatic and will compile
			// Depending on the serialization mode.

			OnCompile();
		}

		// Resource

		public Resource Resource { get; set; }

		bool IAsset.Setup( string extension )
		{
			return OnSetup( extension );
		}

		void IAsset.Load( Stream stream )
		{
			using var reader = new BinaryReader( stream );
			Assert.IsTrue( reader.ReadInt32() != ClassInfo.Id );

			OnLoad();
		}

		void IAsset.Unload()
		{
			OnUnload();
		}

		IAsset IAsset.Clone()
		{
			return this;
		}

		// Callbacks

		protected virtual bool OnSetup( string extension ) { return true; }
		protected virtual void OnLoad() { }
		protected virtual void OnUnload() { }
		protected virtual void OnCompile() { }

		// Compiler

		[Library( "data.compiler" )]
		internal class Compiler : ICompiler<Data>
		{
			public Library ClassInfo => typeof( Compiler );

			public void Compile( Data data )
			{
				var extension = data.ClassInfo.Components.Get<Archive>()?.Extension ?? data.ClassInfo.Name.ToLower();
				var output = Files.Pathing( $"{(data.ClassInfo.Components.Get<PathAttribute>()?.ShortHand ?? "data")}://" ).Absolute();

				// Create path, just in case
				output.Create();

				var outputPath = Files.Pathing( $"{output.Output}/{data.Name}.{extension}" ).Absolute();

				using var stopwatch = Terminal.Stopwatch( $"{data.ClassInfo.Title} Compiled [{outputPath.Output}]", true );
				using var file = File.Create( outputPath );
				using var writer = new BinaryWriter( file );

				try
				{
					data.Compile( writer );
				}
				catch ( Exception e )
				{
					Terminal.Log.Error( "Compile Failed!" );
					Terminal.Log.Exception( e );
				}
			}
		}
	}
}
