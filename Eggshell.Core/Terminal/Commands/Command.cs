using System;

namespace Eggshell.Diagnostics
{
    public interface ICommand
    {
        string Name { get; }
        string Help { get; }

        object Invoke(string[] args);
    }

    public sealed class Command : ICommand
    {
        public string Name { get; }
        public string Help { get; }

        private IParameter[] Arguments { get; }
        private Func<object[], object> Invoker { get; }

        public Command(string name, string help, Func<object[], object> invoker, IParameter[] arguments)
        {
            Invoker = invoker;
            Arguments = arguments;

            Name = name;
            Help = help ?? "n/a";
        }

        public object Invoke(string[] args)
        {
            return Invoker.Invoke(Process(args));
        }

        private object[] Process(string[] args)
        {
            if (Arguments == null || Arguments.Length == 0)
            {
                return Array.Empty<object>();
            }

            // Set all args to lowercase
            for (var i = 0; i < args.Length; i++)
            {
                args[i] = args[i].ToLower();
            }

            var finalArgs = new object[Arguments.Length];

            for (var i = 0; i < Arguments.Length; i++)
            {
                finalArgs[i] = i >= args.Length ? Arguments[i].Default : args[i].Convert(Arguments[i].Type);
            }

            return finalArgs;
        }
    }
}
