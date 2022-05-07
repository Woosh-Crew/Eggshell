using Eggshell;
using Eggshell.Generated;

[assembly : Library]

namespace Eggshell.Tests
{
	public class ConsoleApplication : App
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
