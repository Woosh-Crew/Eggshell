using System.Collections.Generic;

namespace Eggshell.Debugging.Commands
{
	public interface ICommander
	{
		IEnumerable<Command> All { get; }
		
		void Push( Command command );
		object Invoke( string command, string[] args );
	}
}