namespace Eggshell.Reflection
{
	public interface IBinding : IComponent<Library>
	{
		bool OnRegister( ILibrary value )
		{
			return true;
		}
		
		void OnUnregister( ILibrary value ) { }

		ILibrary OnCreate()
		{
			return null;
		}
	}
}
