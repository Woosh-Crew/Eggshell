namespace Eggshell.Reflection
{
    public class DynamicFunction : Function
    {
        public DynamicFunction(string name) : base(name, null) { }

        public override object Invoke(IObject target, params object[] parameters)
        {
            return base.Invoke(target, parameters);
        }
    }
}
