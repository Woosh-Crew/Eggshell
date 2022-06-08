namespace Eggshell
{
    public interface IComponent<T> where T : class
    {
        T Attached { get; set; }
        bool Attachable(T item) { return true; }

        void OnAttached() { }
        void OnDetached() { }
    }
}
