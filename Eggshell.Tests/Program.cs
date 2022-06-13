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

        /// <summary>
        /// This is a test description hello
        /// hello
        /// hello
        /// </summary>
        [ ConVar(Assignable = false)]
        public static float Hello { get; set; }
    }
}
