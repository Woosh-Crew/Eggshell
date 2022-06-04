using System;

namespace Eggshell
{
    /// <summary>
    /// Add this attribute to a class to add it to the library database,
    /// or to override its internal name for identification.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly)]
    public sealed class LibraryAttribute : Attribute
    {
        public string Name { get; }

        public LibraryAttribute() { }

        public LibraryAttribute(string name)
        {
            Name = name;
        }
    }
}
