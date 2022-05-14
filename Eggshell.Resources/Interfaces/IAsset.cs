using System.IO;

namespace Eggshell.Resources
{
	public interface IAsset : ILibrary
	{
		Resource Resource { set; }
		bool Setup( string extension );

		void Load( Stream stream );
		void Unload();

		IAsset Clone();
	}
}
