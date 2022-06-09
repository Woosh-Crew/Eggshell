using System.Collections.Generic;

namespace Eggshell
{
    /// <summary>
    /// Module is Eggshells object oriented application injection. Modules will
    /// be automatically created when you initialize Eggshell
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
        void IModule.OnShutdown() => OnShutdown();

        protected virtual bool OnRegister() { return true; }
        protected virtual void OnReady() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnShutdown() { }
    }
}
