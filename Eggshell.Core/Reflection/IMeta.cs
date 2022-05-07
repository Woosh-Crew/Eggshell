using System.Reflection;

namespace Eggshell.Reflection
{
	public interface IMember<out T> : IMember where T : MemberInfo
	{
		T Info { get; }
	}

	public interface IMember : IMeta
	{
		Library Parent { get; set; }
		int Id { get; }
		bool IsStatic { get; }
	}

	public interface IMeta
	{
		string Name { get; }
		string Help { get; }
		string Title { get; set; }
		string Group { get; set; }
	}
}
