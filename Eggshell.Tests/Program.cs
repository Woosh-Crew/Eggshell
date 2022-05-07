namespace Eggshell.Tests
{
	public class Console : App
	{
		public static void Main( string[] args )
		{
			Crack();
			
			foreach ( var library in Library.Database )
			{
				Terminal.Log.Info( $"{library.Title} - {library.Help}" );
			}
		}
	}
}
