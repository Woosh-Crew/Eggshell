using System;
using Eggshell.IO;

namespace Eggshell
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class PathAttribute : Attribute, IComponent<Library>
    {
        public string ShortHand { get; }
        public string Path { get; }

        public PathAttribute(string shortHand, string path)
        {
            ShortHand = shortHand;
            Path = path;
        }

        public void OnAttached(Library item)
        {
            Pathing.Add(ShortHand, Path);
        }
    }
}
