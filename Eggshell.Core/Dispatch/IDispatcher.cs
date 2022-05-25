using System;
using Eggshell.Reflection;

namespace Eggshell.Dispatching
{
	/// <summary>
	/// IDispatcher is responsible for providing a binder / logic
	/// provider to the Dispatcher itself. This allows you to override
	/// how the dispatcher works.
	/// </summary>
	public interface IDispatcher : IDisposable, ILibrary
	{
		/// <summary>
		/// Add this function to the event stack using this string as
		/// the id it should be invoked from
		/// </summary>
		void Add( string name, Function function );

		/// <summary>
		/// Send out a dispatch by its string id / name without any
		/// parameters.
		/// </summary>
		void Run( string name );
		
		/// <summary>
		/// Send out a dispatch by its string id / name with an array
		/// of parameters.
		/// </summary>
		void Run( string name, params object[] args );

		/// <summary>
		/// Register this object to receive instance based dispatch
		/// messages from the dispatcher.
		/// </summary>
		void Register( ILibrary item );
		
		/// <summary>
		/// Unregister this object to no longer receive dispatch messages,
		/// make sure to call this when destroying an object, or else GC
		/// wont clean it up as it still has references
		/// </summary>
		void Unregister( ILibrary item );
	}
}
