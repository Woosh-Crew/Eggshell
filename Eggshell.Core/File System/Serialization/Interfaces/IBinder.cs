using System.IO;

namespace Eggshell
{
	[Group( "Files" )]
	public interface IBinder : ILibrary
	{
		FileInfo Info { get; set; }
	}
}
