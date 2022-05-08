namespace Eggshell
{
	public interface IModule : ILibrary
	{
		bool OnRegister();

		void OnReady();
		void OnShutdown();
	}
}
