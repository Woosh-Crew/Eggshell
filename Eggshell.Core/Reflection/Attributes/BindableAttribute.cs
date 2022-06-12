using System;

namespace Eggshell.Reflection
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class BindableAttribute : Attribute { }
}
