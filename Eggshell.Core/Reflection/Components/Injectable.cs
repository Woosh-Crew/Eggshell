using Eggshell.Reflection;

namespace Eggshell
{
	/// <summary>
	/// Allows a member to have injected components into it. Basically
	/// builds the source tree to allow custom logic to be injected
	/// </summary>
	[Binding( Type = typeof( Library ) )]
	public partial class Injectable { }
}
