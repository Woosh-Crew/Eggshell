using System;
using System.Collections.Generic;
using System.Diagnostics;
using Eggshell.Diagnostics;

namespace Eggshell.Diagnostics
{
    public interface ILogger
    {
        IReadOnlyCollection<Entry> All { get; }

        void Add(Entry entry);
        void Clear();
    }
}

namespace Eggshell
{
    public static class LoggerExtensions
    {
        /// <summary>
        /// Logs a generic string straight to the active eggshell terminal logger. Will automatically
        /// get stripped if not in debug mode. 
        /// </summary>
        [Conditional("DEBUG"), Conditional("EGGSHELL"), Conditional("LOGGING")]
        public static void Log(this string message)
        {
            Terminal.Log.Info(message);
        }

        /// <summary>
        /// Adds a custom entry to the log output, Useful for debug callbacks to the steamworks api
        /// or any custom framework that has its own logger, and you need to add a custom label to it.
        /// </summary>
        public static void Entry<T>(this ILogger provider, T message, string level, string stack = null)
        {
            provider?.Add(new()
            {
                Message = message?.ToString() ?? "Null",
                Trace = string.IsNullOrWhiteSpace(stack) ? Environment.StackTrace : stack,
                Level = level,
            });
        }

        /// <summary>
        /// Adds a verbose info log to the log output. These are automatically stripped from your
        /// application if you're not doing a debug build.
        /// </summary>
        [Conditional("DEBUG"), Conditional("EGGSHELL"), Conditional("LOGGING")]
        public static void Info<T>(this ILogger provider, T message, string stack = null)
        {
            provider?.Entry(message, "Info", stack);
        }

        /// <summary>
        /// Adds a warning log to the log output. These are automatically stripped from your
        /// application if you're not doing a debug build. (Might change in the future)
        /// </summary>
        [Conditional("DEBUG"), Conditional("EGGSHELL"), Conditional("LOGGING")] // Maybe?
        public static void Warning<T>(this ILogger provider, T message, string stack = null)
        {
            provider?.Entry(message, "Warning", stack);
        }

        /// <summary>
        /// Adds an error log to the log output. These won't be stripped from your application
        /// depending on the build type.
        /// </summary>
        public static void Error<T>(this ILogger provider, T message, string stack = null)
        {
            provider?.Entry(message, "Error", stack);
        }

        /// <summary>
        /// Adds an exception log to the log output. These won't be stripped from your application
        /// depending on the build type.
        /// </summary>
        public static void Exception(this ILogger provider, Exception exception)
        {
            provider?.Add(new()
            {
                Message = $"{exception.Message}",
                Trace = exception.StackTrace,
                Level = "Exception",
            });
        }
    }
}
