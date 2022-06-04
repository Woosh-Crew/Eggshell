using System.Threading.Tasks;
using Eggshell.Coroutines;

namespace Eggshell
{
    public class WaitFor : IYield
    {
        private Task Task { get; }

        public WaitFor(Task task)
        {
            Task = task;
        }

        public bool Wait()
        {
            return Task.IsCompleted;
        }
    }
}
