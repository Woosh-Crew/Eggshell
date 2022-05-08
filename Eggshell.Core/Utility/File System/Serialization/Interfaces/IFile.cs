using System.IO;

namespace Eggshell
{
	[Group( "Files" )]
	public interface IFile : ILibrary
	{
		FileInfo Info { get; set; }
	}
}
