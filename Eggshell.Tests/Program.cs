using System.Collections;

namespace Eggshell.Tests
{
	[Archive( Fallback = "game://testing.txt" )]
	public class Testing : ILibrary
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
			Bootstrap.Update();
			Bootstrap.Update();
			Bootstrap.Update();
			Bootstrap.Update();
			Bootstrap.Update();
			Bootstrap.Update();
			Bootstrap.Update();
			Bootstrap.Update();
		}

		public override void OnReady()
		{
			Terminal.Log.Info( "Hello World" );
			Coroutine.Start( Action() );
		}

		public IEnumerator Action()
		{
			Terminal.Log.Info( "First" );

			yield return null;

			Terminal.Log.Info( "Second" );
		}
	}
}
