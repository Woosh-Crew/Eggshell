using Eggshell.IO;

namespace Eggshell.Resources
{
    public class Origin : IComponent<Resource>
    {
        public Resource Attached { get; set; }
        public Pathing Path { get; set; }
    }
}
