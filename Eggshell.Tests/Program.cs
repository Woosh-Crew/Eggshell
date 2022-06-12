#define EGGSHELL

using Eggshell.IO;
using Eggshell.Resources;

namespace Eggshell.Tests
{
    [Icon(Id = "terminal")]
    public class Console : Project
    {
        static Console()
        {
            Terminal.Command.Push(((Library)typeof(Console)).Properties["console.hello"]);
        }

        public static void Main(string[] args)
        {
            Crack(new());

            Assets.Load<Data>("process://");
            Terminal.Log.Info(((Pathing)"process://").Absolute());
        }

        /// <summary>
        /// This is a test description
        /// </summary>
        public static float Hello { get; set; }
    }
}
