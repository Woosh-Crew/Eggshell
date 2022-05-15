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
}
