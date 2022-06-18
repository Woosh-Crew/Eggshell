#define EGGSHELL

using System;
using System.IO;
using System.Threading.Tasks;
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
        }

        protected async override void OnReady()
        {
            base.OnReady();
        }
    }
}
