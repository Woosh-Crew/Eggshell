using System.IO;

namespace Eggshell.Resources
{
	public interface IAsset : ILibrary
	{
		/// <summary>
		/// Where did this asset come from?
		/// </summary>
		Resource Resource { set; }

		/// <summary>
		/// Gets called on the main asset when setting up the
		/// resource. Use this for assigning file providers
		/// or mutating components based off the file extension
		/// </summary>
		void Setup( string extension );

		void Load( Stream stream );
		void Unload();


		/// <summary>
		/// Clones the asset for use in instances, you can
		/// return itself to make it not use instances.
		/// </summary>
		IAsset Clone();
	}
}
