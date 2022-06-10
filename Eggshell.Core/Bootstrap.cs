using System;
using System.Linq;
using System.Reflection;
using Eggshell.Coroutines;

namespace Eggshell
{
    /// <summary>
    /// Bootstrap is responsible for initializing all eggshell modules for your
    /// application as well as responsible for controlling them. This is overrideable
    /// so you can use this in existing applications / frameworks / engines incredibly easy.
    /// </summary>
    public class Bootstrap : IObject
    {
        public Library ClassInfo { get; }

        /// <summary> 
        /// Creates a new instance of the bootstrap class. This should 
        /// be only constructed one  and used in the Project.Crack to
        /// initialize your application.
        /// </summary>
        public Bootstrap()
        {
            ClassInfo = Library.Register(this);

            if (ClassInfo == null)
            {
                Terminal.Log.Error("Failed to register Bootstrap.");
            }
        }

        internal void Boot()
        {
            OnStart();
            OnModules();

            Ready();
        }

        // Dispatchers
        // --------------------------------------------------------------------------------------- //

        /// <summary>
        /// Invoke ready, to run the OnReady call chain on all modules. You
        /// can override this on the OnBooted method. 
        /// </summary>
        public void Ready()
        {
            OnBooted();
        }

        /// <summary>
        /// Invoke ready, to run the OnFocus call chain on all modules. You
        /// can override this on the OnFocus method. Make sure to call this
        /// in your own bootstrap, as its not called by default! 
        /// </summary>
        public void Focus(bool value)
        {
            OnFocus(value);
        }

        /// <summary>
        /// Invoke Update, to run the update loop on all modules, you
        /// can override this by overriding the protected method OnUpdate;
        /// </summary>
        public void Update()
        {
            OnUpdate();
        }

        /// <summary>
        /// Invoke Shutdown, to run the shutdown call chain on all modules, you
        /// can override this by overriding the protected method OnShutdown
        /// </summary>
        public void Shutdown()
        {
            OnShutdown();
        }

        // Callbacks
        // --------------------------------------------------------------------------------------- //

        /// <summary>
        /// Callbacks specific for the bootstrap, these mostly only get called as
        /// the project is bootstrapping. great for injecting high level logic into
        /// low level code, or for pissing off modules you dont need in your project
        /// </summary>
        public interface Callbacks
        {
            /// <summary>
            /// Everytime a module requests to be created, it'll call this. So you
            /// can override which modules are meant to be initialized at a bootstrap
            /// level. Incredibly useful for removing modules, without breaking anything;
            /// </summary>
            bool OnModule(IModule module);
        }

        // Overrides for Extendability
        // --------------------------------------------------------------------------------------- //

        /// <summary>
        /// OnStart is called by Boot() before any of the modules have been
        /// initialized. Use this for doing pre-initialization.
        /// </summary>
        protected virtual void OnStart() { }

        /// <summary>
        /// Override OnModules to change how modules are cached and registered.
        /// You should never need to override this, but just in case it is.
        /// </summary>
        protected virtual void OnModules()
        {
            // Cache Modules using Reflection
            foreach ( var assembly in AppDomain.CurrentDomain.GetAssemblies() )
            {
                assembly.GetType("Eggshell.Generated.Modules")?
                    .GetMethod("Cache", BindingFlags.Static | BindingFlags.NonPublic)?
                    .Invoke(null, null);
            }
        }

        /// <summary>
        /// Called when eggshell is ready, and the bootstrap has done what it
        /// needs to do. This should call OnReady on all modules, so they can
        /// start there systems.
        /// </summary>
        protected virtual void OnBooted()
        {
            var all = Module._all;

            for (var i = all.Count; i >= 1; i--)
            {
                var running = all[i - 1];

                if (all.Any(e => (e as Callbacks)?.OnModule(running) == false))
                {
                    all.RemoveAt(i - 1);
                    continue;
                }

                running.OnReady();
            }
        }

        /// <summary>
        /// Called when the application changes its focus. Tells all the modules
        /// that the focus has changed.
        /// </summary>
        protected virtual void OnFocus(bool value)
        {
            foreach ( var module in Module.All )
            {
                module.OnFocused(value);
            }
        }

        /// <summary>
        /// The update loop for your application, by default this will call
        /// the OnUpdate method on modules. Shouldn't need to override this.
        /// </summary>
        protected virtual void OnUpdate()
        {
            foreach ( var module in Module.All )
            {
                module.OnUpdate();
            }
        }

        /// <summary>
        /// Called when the application is quitting / shutting down. Use this
        /// to dispose any bootstrap specific resources. Should call OnShutdown
        /// on all modules. 
        /// </summary>
        protected virtual void OnShutdown()
        {
            foreach ( var module in Module.All )
            {
                module.OnShutdown();
            }
        }
    }
}
