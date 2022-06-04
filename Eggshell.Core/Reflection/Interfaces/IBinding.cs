namespace Eggshell.Reflection
{
    /// <summary>
    /// IBinding is a binding to a library. Allows you to inject logic
    /// in to the library pipeline, as well as store custom library meta
    /// data for use in your project. (Such as the Archive Component)
    /// </summary>
    public interface IBinding : IComponent<Library>
    {
        /// <summary>
        /// Should we register this object to the library registry? If
        /// false, it'll destroy itself. If true we should be using this object 
        /// </summary>
        bool OnRegister(IObject value) { return true; }

        /// <summary>
        /// Cleanup any resources when this object gets removed from
        /// the registry (when it is deleted).
        /// </summary>
        void OnUnregister(IObject value) { }

        /// <summary>
        /// What should we do when this object is created through the library
        /// system? Should we use a custom constructor? 
        /// </summary>
        /// <returns></returns>
        IObject OnCreate() { return null; }
    }
}
