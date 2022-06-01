using System.Collections;
using System.Collections.Generic;
using Eggshell.Coroutines;

namespace Eggshell
{
	/// <summary>
	/// Coroutines are methods that can be held / waited / yielded
	/// half way through it, and can sequentially perform actions over a
	/// period of time, while being in the same callstack.
	/// </summary>
	public class Coroutine : Module
	{
		// Coroutine API
		// --------------------------------------------------------------------------------------- //

		/// <summary>
		/// Adds a new coroutine to the coroutine stack. A coroutine function must
		/// return IEnumerator (I know its weird) in order for it to work properly
		/// </summary>
		/// <param name="enumerator"></param>
		public static void Start( IEnumerator enumerator )
		{
			enumerator.MoveNext();

			Get<Coroutine>().Running.Add( enumerator );
		}

		/// <summary>
		/// Removes a running coroutine from the coroutine stack (If you saved a
		/// reference to it!) Make sure to not call this while in a coroutine, as
		/// it might break it.
		/// </summary>
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
