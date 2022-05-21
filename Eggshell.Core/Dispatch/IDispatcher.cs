using System;
using Eggshell.Reflection;

namespace Eggshell.Dispatching
{
	public interface IDispatcher : IDisposable
	{
		void Add( string eventName, Function function );

		void Run( string name );
		void Run( string name, params object[] args );

		void Register( ILibrary item );
		void Unregister( ILibrary item );
	}
}
