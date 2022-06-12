using Eggshell.Reflection;

namespace Eggshell
{
    [Binding(Type = typeof(Function))]
    public partial class ConCmd
    {
        public void OnAttached()
        {
            Terminal.Command.Push(Attached);
        }
    }
    
    [Binding(Type = typeof(Property))]
    public partial class ConVar
    {
        public void OnAttached()
        {
            Terminal.Command.Push(Attached);
        }
    }
}
