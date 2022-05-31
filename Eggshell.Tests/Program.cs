#define EGGSHELL

using Eggshell.Reflection;

namespace Eggshell.Tests
{
	[Archive( Fallback = "game://testing.txt" )]
	public class Testing : IObject
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
		}
	}
}
