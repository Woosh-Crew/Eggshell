namespace Eggshell
{
	public interface IModule : ILibrary
	{
		bool OnRegister();

		void OnReady();
		void OnUpdate();
		void OnShutdown();
	}
}
