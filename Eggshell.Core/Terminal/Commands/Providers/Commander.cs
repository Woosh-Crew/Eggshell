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

        public void Pull(string command)
        {
            _commands.Remove(command);
        }

        public object Invoke(string command, string[] args)
        {
            if (!_commands.TryGetValue(command, out var value))
            {
                // No Command

                Terminal.Log.Entry($"Couldn't find command \"{command}\"", Terminal.Level);
                return null;
            }

            if (args.Contains("/?"))
            {
                // Log Help

                Terminal.Log.Entry($"[{value.Name}] = {value.Help}", Terminal.Level);
                return null;
            }

            return value.Invoke(args);
        }
    }
}
