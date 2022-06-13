using System;
using System.Diagnostics;
using System.Linq;
using Eggshell.Diagnostics;

namespace Eggshell
{
    /// <summary>
    /// Eggshell's core Debugging Library. Has support for
    /// logging, commands, overlays, and other utility features.
    /// </summary>
    [Link("debug.terminal"), Group("Debug")]
    public class Terminal : Module
    {
        public const string Level = "Terminal";

        /// <summary>
        /// Command Console. Use Run(string, object[]) to run a command.
        /// Your game can have its own Console provider.
        /// </summary>
        public static ICommander Command { get; set; }

        /// <summary>
        /// Add your own extension methods if need be, since this is an
        /// instanced class. Usually controlled by the bootstrap
        /// </summary>
        public static ILogger Log { get; set; }

        /// <summary>
        /// Overlays are things rendered over everything that represent
        /// debug information. This is usually null and depends on what the
        /// bootstrap does to initialize it.
        /// </summary>
        public static IOverlays Overlays { get; set; }

        /// <summary>
        /// Returns true if the instance was launched in developer mode,
        /// meaning there was a launch arg of -dev.
        /// </summary>
        [Link("app.debug"), ConVar]
        public static bool IsDebug { get; }

        /// <summary>
        /// Was the running process launched in a headless way (meaning no
        /// graphics)? Generated by the command line arguments
        /// </summary>
        [Link("app.headless"), ConVar]
        public static bool IsHeadless { get; }

        /// <summary>
        /// Returns true if the instance was launched in some sort of editor
        /// that can be used to edit assets and compile them. Set by the bootstrap
        /// (Such as launching Eggshell in the Unity Editor)
        /// </summary>
        [Link("app.editor"), ConVar(Assignable = false)]
        public static bool IsEditor { get; set; }

        /// <summary>
        /// Should we report the stopwatch logs, or any other terminal
        /// specific things.
        /// </summary>
        [Link("debug.report"), ConVar]
        public static bool Report { get; set; } = true;

        /// <summary>
        /// Runs a stopwatch on a IDisposable Scope. Use this in a using() expression
        /// to record how long it took to execute that code block.
        /// </summary>
        public static IDisposable Stopwatch(string message = null, bool alwaysReport = false)
        {
            return Report || alwaysReport ? new TimedScope(message) : null;
        }

        static Terminal()
        {
            var args = Environment.GetCommandLineArgs();

            // Setup Args

            IsDebug = args.Contains("-dev") || args.Contains("-debug") || args.Contains("-editor");
            IsHeadless = args.Contains("-headless") || args.Contains("-batchmode") || args.Contains("-batch") || args.Contains("-cmd");

            Log = new ConsoleLogger();
            Command = new Commander();

            // Push default commands

            Command.Push("dump", Dump, "Dumps all library meta data into the terminal");

            Command.Push<string>("help", Help, "Dumps all commands into the terminal");
            Command.Push<string>("exec", Execute, "Executes all commands within a text file (per line)");
        }

        private static void Help(string input = null)
        {
            foreach ( var command in Command.All )
            {
                Log.Entry($"[{command.Name}] = {command.Help}", Level);
            }
        }

        private static void Dump()
        {
            foreach ( var library in Library.Database )
            {
                Log.Entry($"[{library.Name}] - {library.Help}", Level);
                foreach ( var property in library.Properties )
                {
                    Log.Entry($"\t[{property.Name}] - {property.Help}", Level);
                }
            }
        }

        private static void Execute(string input = null)
        {
            if (input == null)
            {
                Log.Error("No Path provided in args.");
                return;
            }

            Command.Execute(input);
        }

        protected override void OnReady()
        {
            var args = Environment.GetCommandLineArgs();

            // Invoke cmd from the command line args
            foreach ( var invoke in args.Where(e => e.Contains('+')) )
            {
                var input = invoke.Substring(1);

                Log.Entry($"> {input}", Level);
                Command.Invoke(input);
            }
        }

        // Debug

        private class TimedScope : IDisposable
        {
            private readonly Stopwatch _stopwatch;
            private readonly string _message;

            public TimedScope(string message)
            {
                _message = message;

                _stopwatch = System.Diagnostics.Stopwatch.StartNew();
            }

            public void Dispose()
            {
                _stopwatch.Stop();

                var time = _stopwatch.Elapsed.Seconds > 0 ? $"{_stopwatch.Elapsed.TotalSeconds} seconds" : $"{_stopwatch.Elapsed.TotalMilliseconds} ms";

                if (string.IsNullOrEmpty(_message))
                {
                    Log.Info(time);
                    return;
                }

                Log.Info($"{_message} | {time}");
            }
        }
    }
}
