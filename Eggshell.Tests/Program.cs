namespace Eggshell.Tests
{
	public class Console : Project
	{
		public static void Main( string[] args )
		{
			Crack( new() );
		}

		[Dispatch( "eggshell.ready" )]
		public static void Testing() { }
	}
}
