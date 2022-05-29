using System;
using Eggshell.Coroutines;

namespace Eggshell
{
	public class WaitForSeconds : IYield
	{
		private float _seconds;

		public float Seconds
		{
			get => _seconds;
			set
			{
				_seconds = value;
			}
		}

		public WaitForSeconds( float seconds )
		{
			Seconds = seconds;
		}

		public bool Wait()
		{
			// Not implemented just yet!
			return true;
		}
	}

	public class WaitUntil : IYield
	{
		private Func<bool> Waiting { get; }

		public WaitUntil( Func<bool> until )
		{
			Waiting = until;
		}

		public bool Wait()
		{
			return !Waiting.Invoke();
		}
	}
}
