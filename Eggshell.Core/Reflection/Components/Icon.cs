using Eggshell.Reflection;

namespace Eggshell
{
	/// <summary>
	/// An icon that represents a class. Useful for GUI where a file
	/// would usually need an icon (like .fbx or .wav, etc).
	/// </summary>
	[Binding( Type = typeof( Library ) )]
	public partial class Icon
	{
		/// <summary>
		/// What icon id should we load. (Depends on the application but
		/// it can be pulled from (for instance) google material icons)
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// What icon file should we load from disk. If ID is set, it will
		/// prioritise that over the path.
		/// </summary>
		public string Path { get; set; }
	}
}
