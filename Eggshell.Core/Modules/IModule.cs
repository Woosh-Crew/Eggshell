namespace Eggshell
{
    /// <summary>
    /// Modules are automatically created by eggshell from the bootstrap
    /// on application startup (when you crack the egg). Modules allow incredibly
    /// easy dependency injection, as well as easily creating scalable applications 
    /// </summary>
    public interface IModule : IObject
    {
        /// <summary>
        /// Tells the bootstrap if we should initialize this module
        /// and add it to the module stack, if no will just unregister
        /// </summary>
        bool OnRegister();

        /// <summary>
        /// The module is now ready, has been added to the stack and
        /// other modules have been created. What should we do now?
        /// </summary>
        void OnReady();

        /// <summary>
        /// The application loop of a module, usually called every
        /// loop. Depends on the bootstrap. Use this for API callbacks
        /// and other constantly occuring events
        /// </summary>
        void OnUpdate();

        /// <summary>
        /// A callback for when the application has changed its focus
        /// value. Gets called when the application looses or gains
        /// focus. Use this for pausing operations, if need be.
        /// </summary>
        void OnFocused(bool focused);

        /// <summary>
        /// Called when the application is shutting down, or the module
        /// is being removed from the stack. Make sure to unregister
        /// and dispose everything this module made from this function!
        /// </summary>
        void OnShutdown();
    }
}
