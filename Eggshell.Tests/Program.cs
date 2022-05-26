namespace Eggshell.Tests
{
	[Archive( Fallback = "game://testing.txt" )]
	public class Testing : ILibrary
	{
		public Library ClassInfo { get; }

		public Testing()
		{
			ClassInfo = Library.Register( this );
		}
	}

	[Icon( Id = "terminal" )]
	public class Console : Project
	{
		public static void Main( string[] args )
		{
			Crack( new() );
			
			Terminal.Log.Info( "Wassup" );
		}
	}
}
