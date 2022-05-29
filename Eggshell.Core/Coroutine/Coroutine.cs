using System;
using System.Collections;
using System.Collections.Generic;
using Eggshell.Coroutines;

namespace Eggshell
{
	public class Coroutine : Module
	{
		// Coroutine API
		// --------------------------------------------------------------------------------------- //

		public static void Start( IEnumerator enumerator )
		{
			enumerator.MoveNext();

			Get<Coroutine>().Running.Add( enumerator );
		}

		public static void Remove( IEnumerator enumerator )
		{
			Get<Coroutine>().Running.Remove( enumerator );
		}

		// Internal Module
		// --------------------------------------------------------------------------------------- //

		private List<IEnumerator> Running { get; } = new();

		public override void OnUpdate()
		{
			for ( var i = Running.Count; i > 0; i-- )
			{
				var running = Running[i - 1];

				if ( (!(running.Current as IYield)?.Wait() ?? true) && !running.MoveNext() )
				{
					Running.RemoveAt( i - 1 );
				}
			}
		}
	}
}
