using System;
using System.Collections.Generic;

namespace Eggshell.Diagnostics
{
    public sealed class ConsoleLogger : ILogger
    {
        public IReadOnlyCollection<Entry> All => _logs;
        private readonly List<Entry> _logs = new();

        // Logs

        public void Add(Entry entry)
        {
            Console.BackgroundColor = ConsoleColor.Black;

            if (string.IsNullOrEmpty(entry.Message))
            {
                entry.Message = "n/a";
            }

            if (entry.Level.Contains("Warn"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }

            if (entry.Level.Contains("Error") || entry.Level.Contains("Exception"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            entry.Message = $"{entry.Message}";
            entry.Time = DateTime.Now;

            Console.WriteLine($"[{entry.Time.ToShortTimeString()}] [{entry.Level}] {entry.Message}");
            Console.ResetColor();

            if (entry.Level.Contains("Error") || entry.Level.Contains("Exception"))
            {
                Console.WriteLine(entry.Trace);
            }

            _logs.Add(entry);
        }

        public void Clear()
        {
            _logs.Clear();
            Console.Clear();
        }
    }
}
