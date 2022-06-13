using System;

namespace Eggshell
{
    /// <summary>
    /// Add this attribute to a class or any Library Reflection type
    /// to add it to the library database, or to override its internal
    /// name for identification.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class LinkAttribute : Attribute
    {
        public string Name { get; }

        public LinkAttribute() { }

        public LinkAttribute(string name)
        {
            Name = name;
        }
    }
}
