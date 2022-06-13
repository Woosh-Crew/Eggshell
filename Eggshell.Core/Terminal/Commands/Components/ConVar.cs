using Eggshell.Reflection;

namespace Eggshell
{
    [Binding(Type = typeof(Property))]
    public partial class ConVar
    {
        public bool Assignable { get; set; }

        public void OnAttached()
        {
            if (!Attached.IsStatic)
            {
                Terminal.Log.Warning($"Property [{Attached.Name}] needs to be static inorder for it to be a ConVar");
                return;
            }

            Terminal.Command.Push(Attached, !Assignable);
        }
    }
}
