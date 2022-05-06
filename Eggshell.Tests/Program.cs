using System;

namespace Eggshell.Tests
{
	public class ConsoleApplication : App
	{
		public static void Main( string[] args )
		{
			Crack();
			
			Console.WriteLine( "Hello World!" );
			foreach ( var module in All )
			{
				Console.WriteLine( module.ToString() );
			}
		}
	}
}
