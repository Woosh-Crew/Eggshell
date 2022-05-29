using System.IO;

namespace Eggshell
{
	[Group( "Files" )]
	public interface IBinder : IObject
	{
		FileInfo Info { get; set; }
	}
}
