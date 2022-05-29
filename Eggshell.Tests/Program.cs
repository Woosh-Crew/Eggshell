#define EGGSHELL

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

			// Update
			Bootstrap.Update();
			Bootstrap.Update();
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
			Coroutine.Start( Action() );
		}

		public override void OnUpdate()
		{
			_index++;
		}

		private int _index;

		public IEnumerator Action()
		{
			Terminal.Log.Info( "Starting" );

			_index = 0;

			yield return new WaitUntil( () => _index == 5 );
			Terminal.Log.Info( $"We're at {_index}" );

			yield return new WaitUntil( () => _index == 10 );
			Terminal.Log.Info( $"We're at {_index}" );
		}
	}
}
