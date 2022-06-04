using System;
using Eggshell.Coroutines;

namespace Eggshell
{
    public class WaitUntil : IYield
    {
        private Func<bool> Waiting { get; }

        public WaitUntil(Func<bool> until)
        {
            Waiting = until;
        }

        public bool Wait()
        {
            return !Waiting.Invoke();
        }
    }
}
