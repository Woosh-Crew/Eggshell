#define EGGSHELL

using Eggshell.IO;
using Eggshell.Resources;

namespace Eggshell.Tests
{
    [Icon(Id = "terminal")]
    public class Console : Project
    {
        public static void Main(string[] args)
        {
            Crack(new());
            
            // Assets.Load<Data>("process://");
            Terminal.Log.Info(((Pathing)"process://").Absolute());
        }
    }
}
