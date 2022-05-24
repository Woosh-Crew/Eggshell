namespace Eggshell.Tests
{
	[Archive( "", Fallback = "game://testing.txt" ), Constructor( "return Eggshell.Tests.Testing.Create( this );" )]
	public class Testing : ILibrary
	{
		public Library ClassInfo { get; }

		public Testing()
		{
			ClassInfo = Library.Register( this );
		}

		public static ILibrary Create( Library library )
		{
			return new Testing();
		}
	}

	[Icon( Id = "terminal" )]
	public class Console : Project
	{
		public static void Main( string[] args )
		{
			Crack( new() );

			var testing = new Testing();
			Terminal.Log.Info( testing.ClassInfo.Components.Get<Archive>() );
			Terminal.Log.Info( testing.ClassInfo.Help );
		}

		[Dispatch( "eggshell.ready" )]
		public static void Testing() { }
	}
}
