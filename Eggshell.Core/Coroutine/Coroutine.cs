using System;
using System.Collections;
using System.Collections.Generic;

namespace Eggshell
{
	public class Wait : IEnumerator
	{
		public object Current { get; }

		public bool MoveNext()
		{
			return false;
		}

		public void Reset() { }
	}

	public class Coroutine : Module
	{
		// Coroutine API
		// --------------------------------------------------------------------------------------- //

		public static void Start( IEnumerator enumerator )
		{
			enumerator.MoveNext();

			Get<Coroutine>().Running.Add( enumerator );
		}

		public static void Start( Func<IEnumerator> func )
		{
			var enumerator = func.Invoke();
			enumerator.MoveNext();

			Get<Coroutine>().Running.Add( enumerator );
		}

		// Internal Module
		// --------------------------------------------------------------------------------------- //

		private List<IEnumerator> Running { get; } = new();

		public override void OnUpdate()
		{
			base.OnUpdate();

			for ( var i = Running.Count; i > 0; i-- )
			{
				var remove = Running[i - 1].MoveNext();

				if ( remove )
				{
					Running.RemoveAt( i - 1 );
				}
			}
		}
	}
}
