using System.Collections.Generic;

namespace Eggshell
{
    /// <summary>
    /// Modules are automatically created by eggshell from the bootstrap
    /// on application startup (when you crack the egg). Modules allow incredibly
    /// easy dependency injection, as well as easily creating scalable applications 
    /// </summary>
    [Singleton]
    public abstract class Module : IModule
    {
        private static IModule _cached;
        private static readonly List<IModule> _all = new();

        // Static API
        // --------------------------------------------------------------------------------------- //

        public static IEnumerable<IModule> All => _all;

        /// <summary>
        /// Try's to get a module by its type and will return it if it could
        /// find it in the registry. Will return null if it couldn't find it.
        /// </summary>
        public static T Get<T>() where T : class, IModule
        {
            if (_cached is T cached)
            {
                return cached;
            }

            for (var i = 0; i < _all.Count; i++)
            {
                if (_all[i] is not T comp)
                    continue;

                _cached = comp;
                return comp;
            }

            return null;
        }

        /// <summary>
        /// Try's to create a module by its type and will add it to the registered
        /// list if it successes in doing so. Calls to this function will automatically
        /// be created by source generators, and invoked when the egg is cracked.
        /// </summary>
        public static void Create<T>() where T : class, IModule, new()
        {
            var item = new T();

            if (item.ClassInfo == null)
            {
                Terminal.Log.Warning("ClassInfo for Module was null");
                return;
            }

            if (!item.OnRegister() || !Project.Bootstrap.OnValidate(item))
            {
                return;
            }

            _all.Add(item);
        }

        // Instance
        // --------------------------------------------------------------------------------------- //

        public Library ClassInfo { get; }

        public Module()
        {
            ClassInfo = Library.Register(this);
            Assert.IsNull(ClassInfo);
        }

        bool IModule.OnRegister() => OnRegister();
        void IModule.OnReady() => OnReady();
        void IModule.OnUpdate() => OnUpdate();
        void IModule.OnFocused(bool value) => OnFocused(value);
        void IModule.OnShutdown() => OnShutdown();

        /// <summary>
        /// Tells the bootstrap if we should initialize this module
        /// and add it to the module stack, if no will just unregister
        /// </summary>
        protected virtual bool OnRegister() { return true; }

        /// <summary>
        /// The module is now ready, has been added to the stack and
        /// other modules have been created. What should we do now?
        /// </summary>
        protected virtual void OnReady() { }

        /// <summary>
        /// The application loop of a module, usually called every
        /// loop. Depends on the bootstrap. Use this for API callbacks
        /// and other constantly occuring events
        /// </summary>
        protected virtual void OnUpdate() { }

        /// <summary>
        /// A callback for when the application has changed its focus
        /// value. Gets called when the application looses or gains
        /// focus. Use this for pausing operations, if need be.
        /// </summary>
        protected virtual void OnFocused(bool focused) { }

        /// <summary>
        /// Called when the application is shutting down, or the module
        /// is being removed from the stack. Make sure to unregister
        /// and dispose everything this module made from this function!
        /// </summary>
        protected virtual void OnShutdown() { }
    }
}
