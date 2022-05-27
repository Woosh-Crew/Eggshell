using Eggshell.Reflection;

namespace Eggshell
{
	/// <summary>
	/// Allows a member to have injected components into it. Basically
	/// builds the source tree to allow custom logic to be injected
	/// </summary>
	public class Injectable : IBinding
	{
		public void OnAttached( Library item ) { }
	}
}
