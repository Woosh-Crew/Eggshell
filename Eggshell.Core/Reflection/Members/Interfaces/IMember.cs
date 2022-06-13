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
        void OnAttached(Library library);

        int Id { get; }
        bool IsStatic { get; }
    }
}
