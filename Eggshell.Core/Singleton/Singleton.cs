using Eggshell.Reflection;

namespace Eggshell
{
	public class Singleton : IBinding
	{
		public Library Owner { get; private set; }
		public ILibrary Instance { get; private set; }

		// IComponent

		public void OnAttached( Library item )
		{
			Owner = item;
		}

		// IBinding

		public ILibrary OnCreate()
		{
			return Instance;
		}

		public bool OnRegister( ILibrary value )
		{
			if ( Instance != null )
			{
				Terminal.Log.Error( $"You're trying to register another Singleton [{Owner.Name}]" );
				return false;
			}

			Instance = value;
			return true;
		}

		public void OnUnregister( ILibrary value )
		{
			if ( Instance == value )
			{
				Instance = null;
			}
		}
	}
}
