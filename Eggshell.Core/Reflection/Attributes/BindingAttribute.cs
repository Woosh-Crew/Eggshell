using System;

namespace Eggshell.Reflection
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class BindingAttribute : Attribute
    {
        public Type Type { get; set; }
    }
}
