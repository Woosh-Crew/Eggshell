using System;
using System.Collections.Generic;
using System.Linq;

namespace Eggshell.Diagnostics
{
    internal sealed class Commander : ICommander
    {
        public IEnumerable<ICommand> All => _commands.Values;
        private readonly Dictionary<string, ICommand> _commands = new(StringComparer.CurrentCultureIgnoreCase);

        public void Push(ICommand command)
        {
            _commands.Add(command.Name, command);
        }

        public object Invoke(string command, string[] args)
        {
            if (!_commands.TryGetValue(command, out var value))
            {
                // No Command
                
                Terminal.Log.Info($"Couldn't find command \"{command}\"");
                return null;
            }

            if (args.Contains("/?"))
            {
                // Log Help
                
                Terminal.Log.Info($"[{value.Name}] = {value.Help}");
                return null;
            }
           
            return  value.Invoke(args);;
        }
    }
}
