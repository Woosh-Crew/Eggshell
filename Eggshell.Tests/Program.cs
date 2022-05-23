namespace Eggshell.Tests
{
	[Archive( Extension = "balls" )]
	public class Testing : ILibrary
	{
		public Library ClassInfo { get; }

		public Testing()
		{
			ClassInfo = Library.Register( this );
		}
	}


	public class Console : Project
	{
		public static void Main( string[] args )
		{
			Crack( new() );

			var testing = new Testing();
			Terminal.Log.Info( testing.ClassInfo.Components.Get<Archive>().Extension );
		}

		[Dispatch( "eggshell.ready" )]
		public static void Testing() { }
	}
}
