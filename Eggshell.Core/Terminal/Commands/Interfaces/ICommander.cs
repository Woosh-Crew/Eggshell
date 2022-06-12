using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Eggshell.Diagnostics;
using Eggshell.IO;
using Eggshell.Reflection;

namespace Eggshell.Diagnostics
{
    public interface ICommander
    {
        IEnumerable<ICommand> All { get; }

        void Push(ICommand command);
        object Invoke(string command, string[] args);
    }
}

namespace Eggshell
{
    public static class CommanderExtensions
    {
        public static void Push<T>(this ICommander commander, string name, Func<T> function, string help = null)
        {
            object Invoker(object[] _) => function.Invoke();

            commander.Push(new Command(name, help, Invoker, null));
        }

        public static void Push(this ICommander commander, Function function)
        {
            var args = function.Info.GetParameters();
            var arguments = new IParameter[args.Length];

            for (var i = 0; i < args.Length; i++)
            {
                arguments[i] = new Parameter(args[i].DefaultValue, args[i].ParameterType);
            }

            commander.Push(new Command(function.Name, function.Help, function.Invoke, arguments));
        }

        public static void Push(this ICommander commander, Property property)
        {
            object Invoker(object[] e)
            {
                if (e[0] == null)
                {
                    Terminal.Log.Info($"Property [{property.Name}] is equal to [{property.Value}]");
                    return property.Value;
                }

                property.Value = e[0];
                Terminal.Log.Info($"Property [{property.Name}] now equals [{property.Value}]");
                return property.Value;
            }

            var arguments = new IParameter[]
            {
                new Parameter(null, property.Type)
            };

            commander.Push(new Command(property.Name, property.Help, Invoker, arguments));
        }

        public static void Push(this ICommander commander, string name, Action function, string help = null)
        {
            object Invoker(object[] _)
            {
                function?.Invoke();
                return null;
            }

            commander.Push(new Command(name, help, Invoker, null));
        }

        public static void Push<T>(this ICommander commander, string name, Action<T> function, string help = null)
        {
            object Invoker(object[] args)
            {
                function?.Invoke((T)args[0]);
                return null;
            }

            commander.Push(new Command(name, help, Invoker, new IParameter[]
            {
                new Parameter(null, typeof(T))
            }));
        }

        public static void Push<T1, T2>(this ICommander commander, string name, Action<T1, T2> function, string help = null)
        {
            object Invoker(object[] args)
            {
                function?.Invoke((T1)args[0], (T2)args[1]);
                return null;
            }

            commander.Push(new Command(name, help, Invoker, new IParameter[]
            {
                new Parameter(null, typeof(T1)),
                new Parameter(null, typeof(T2))
            }));
        }

        public static void Push<T1, T2, T3>(this ICommander commander, string name, Action<T1, T2, T3> function, string help = null)
        {
            object Invoker(object[] args)
            {
                function?.Invoke((T1)args[0], (T2)args[1], (T3)args[1]);
                return null;
            }

            commander.Push(new Command(name, help, Invoker, new IParameter[]
            {
                new Parameter(null, typeof(T1)),
                new Parameter(null, typeof(T2)),
                new Parameter(null, typeof(T3))
            }));
        }

        public static void Invoke(this ICommander provider, string commandLine)
        {
            foreach ( var targetCommand in commandLine.Split(';') )
            {
                var name = targetCommand.TrimStart().Split(' ').First();
                var args = targetCommand.Substring(name.Length).SplitArguments();

                // Invoke multiple commands at the same time
                provider.Invoke(name, args);
            }
        }

        public static T Invoke<T>(this ICommander provider, string command, string[] args)
        {
            return (T)provider.Invoke(command, args);
        }

        public static T Invoke<T>(this ICommander provider, string command)
        {
            var name = command.TrimStart().Split(' ').First();
            var args = command.Substring(name.Length).SplitArguments();

            return (T)provider.Invoke(name, args);
        }

        public static void Execute(this ICommander commander, Pathing pathing)
        {
            // Per-Line is a invokable

            foreach ( var line in File.ReadAllLines(pathing.Absolute()) )
            {
                commander.Invoke(line);
            }
        }
    }
}
