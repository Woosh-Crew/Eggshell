using System;
using Eggshell.IO;

namespace Eggshell
{
	/// <summary>
	/// Reflection Component for storing data about file specific meta.
	/// </summary>
	[AttributeUsage( AttributeTargets.Class, Inherited = false )]
	public sealed class FileAttribute : Attribute, IComponent<Library>
	{
		/// <summary>
		/// What file should we load by default if loading one failed.
		/// </summary>
		public string Fallback { get; set; }

		/// <summary>
		/// The default extension for this file.
		/// </summary>
		public string Extension { get; set; }

		/// <summary>
		/// The serialization type for this file.
		/// </summary>
		public Serialization Serialization { get; set; }

		public void OnAttached( Library library ) { }
	}
}
