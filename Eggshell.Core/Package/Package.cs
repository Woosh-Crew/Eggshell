using Eggshell.Reflection;

namespace Eggshell
{
    public class Package : IObject, IMeta
    {
        public Library ClassInfo { get; }

        public Package()
        {
            ClassInfo = Library.Register(this);
        }

        // IMeta

        public string Name { get; set; }
        public string Title { get; set; }
        public string Help { get; set; }
        public string Group { get; set; }
    }
}
