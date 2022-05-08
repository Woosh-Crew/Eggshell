using System.Reflection;
using Eggshell.Reflection;

namespace Eggshell.Tests
{
	public class Console : App
	{
		public string Variable { get; set; }

		public static void Main( string[] args )
		{
			var console_type = typeof( Console );
			var lib = new Library( "console", console_type )
			{
				Title = "Console",
				Group = "Tests",
				Help = "Hello World"
			};

			lib.Properties.Add( new Property( "prop.variable", console_type.GetProperty( "Variable", BindingFlags.Public ) ) );

			Crack();

			foreach ( var library in Library.Database )
			{
				Terminal.Log.Info( $"{library.Title} - {library.Help}" );
			}
		}
	}
}
