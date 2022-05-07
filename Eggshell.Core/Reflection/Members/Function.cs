using System.Reflection;

namespace Eggshell.Reflection
{
	public class Function : ILibrary, IMember<MethodInfo>
	{
		public Library ClassInfo { get; }
		public string Name { get; }
		public string Help { get; }
		public string Title { get; set; }
		public string Group { get; set; }
		public Library Parent { get; set; }
		public int Id { get; }
		public bool IsStatic { get; }
		public MethodInfo Info { get; }
	}
}
