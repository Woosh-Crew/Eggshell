using Eggshell.IO;

namespace Eggshell.Resources
{
	public class Origin : IComponent<Resource>
	{
		public Pathing Path { get; set; }
		public void OnAttached( Resource item ) { }
	}
}
