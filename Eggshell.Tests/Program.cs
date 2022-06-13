#define EGGSHELL

using System;
using Eggshell.Reflection;

namespace Eggshell.Tests
{
    [Icon(Id = "terminal")]
    public class Console : Project
    {
        public Components<Console> Components { get; }

        public Console()
        {
            Components = new(this);
        }

        public static void Main(string[] args)
        {
            Crack(new());

            Dispatch.Run("event.hello2");
        }

        [Dispatch("event.hello2")]
        public static void Hello2() { }

        /// <summary>
        /// This is a test description
        /// </summary>
        [ConVar(Assignable = false)]
        public static float Hello { get; set; }
    }
}
