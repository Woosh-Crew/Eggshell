#define EGGSHELL

using System;
using System.IO;
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

        protected override void OnReady()
        {
        }
    }

}
