using Eggshell.Reflection;

namespace Eggshell
{
    [Binding(Type = typeof(Function))]
    public partial class ConCmd
    {
        public void OnAttached()
        {
            if (!Attached.IsStatic)
            {
                Terminal.Log.Warning($"Function [{Attached.Name}] needs to be static inorder for it to be a ConCmd");
                return;
            }
            
            Terminal.Command.Push(Attached);
        }
    }
}
