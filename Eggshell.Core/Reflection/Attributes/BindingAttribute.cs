using System;

namespace Eggshell.Reflection
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BindingAttribute : Attribute
    {
        public Type Type { get; set; }
    }
}
