using Eggshell.Coroutines;

namespace Eggshell
{
	public class WaitForSeconds : IYield
	{
		public float Seconds { get; set; }

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
}
