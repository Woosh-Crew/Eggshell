namespace Eggshell.Tests
{
	public class ConsoleApplication 
	{
		public static void Main( string[] args )
		{
			foreach ( var library in Library.Database )
			{
				Terminal.Log.Info( $"{library.Title} - {library.Help}" );
			}
		}
	}
}
