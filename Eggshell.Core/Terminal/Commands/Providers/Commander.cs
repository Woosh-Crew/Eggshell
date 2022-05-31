using System;
using System.Collections.Generic;

namespace Eggshell.Diagnostics
{
	internal sealed class Commander : ICommander
	{
		public IEnumerable<Command> All => _commands.Values;
		private readonly Dictionary<string, Command> _commands = new( StringComparer.CurrentCultureIgnoreCase );

		public void Push( Command command )
		{
			_commands.Add( command.Member.Name, command );
		}

		public object Invoke( string command, string[] args )
		{
			if ( !_commands.TryGetValue( command, out var consoleCommand ) )
			{
				Terminal.Log.Info( $"Couldn't find command \"{command}\"" );
				return null;
			}

			consoleCommand.Invoke( Command.ConvertArgs( consoleCommand.Info, args ) );

			return null;
		}
	}
}
