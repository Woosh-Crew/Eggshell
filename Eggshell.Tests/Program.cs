using Eggshell;

[assembly : Library]

namespace Eggshell.Tests
{
	/// <summary>
	/// Hello World
	/// </summary>
	public class ConsoleApplication : App
	{
		public static void Main( string[] args )
		{
			Crack();

			foreach ( var library in Library.Database )
			{
				Terminal.Log.Info( $"{library.Title} - {library.Help}");
			}
		}
	}
}
