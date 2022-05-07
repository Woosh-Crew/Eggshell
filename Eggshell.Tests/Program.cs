using Eggshell.Reflection;

namespace Eggshell.Tests
{
	public class Console : App
	{
		public static void Main( string[] args )
		{
			// var console_type = typeof( Console );
			// var lib = new Library( "console", console_type )
			// {
			// 	Properties = new()
			// 	{
			// 		new Property( "Hello", console_type.GetProperty( "Hello" ) ),
			// 	}
			// };

			Crack();

			foreach ( var library in Library.Database )
			{
				Terminal.Log.Info( $"{library.Title} - {library.Help}" );
			}
		}
	}
}
