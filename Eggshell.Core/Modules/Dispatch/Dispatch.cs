using System;
using Eggshell.Dispatching;
using Eggshell.Reflection;

namespace Eggshell
{
    /// <summary>
    /// Fire and forget message dispatching. Primarily used for invoking
    /// methods from a string (string based events) can be used across
    /// languages. (C# - C++, vice versa)
    /// </summary>
    [Binding(Type = typeof(Function))]
    public partial class Dispatch
    {
        /// <summary>
        /// The hook that gets attached to a library, that allows that library
        /// to receive callbacks from the dispatcher.
        /// </summary>
        private class Hook : Library.Binding
        {
            public Library Attached { get; set; }

            public bool OnRegister(IObject value)
            {
                Register(value);
                return true;
            }

            public void OnUnregister(IObject value)
            {
                Unregister(value);
            }
        }

        /// <summary>
        /// The event name that the dispatcher is looking for when invoking
        /// a dispatch event.
        /// </summary>
        [Skip] public string Event { get; }

        public Dispatch(string target)
        {
            Event = target;
        }

        public void OnAttached()
        {
            // Add it to the Provider
            Provider.Add(Event, Attached);

            if (!Attached.Parent.Components.Has<Hook>())
            {
                // Register class to have hooks
                Attached.Parent.Components.Create<Hook>();
            }
        }

        // Dispatch API
        // --------------------------------------------------------------------------------------- //

        /// <summary>
        /// The underlying system that controls the dispatcher. Override
        /// this to add your own dispatching logic.
        /// </summary>
        public static IDispatcher Provider { get; set; } = new Dispatcher();

        /// <summary>
        /// Runs a fire and forget dispatch. Incredibly useful for global
        /// events, where you don't need to create boiler plate code to setup 
        /// </summary>
        public static void Run(string name)
        {
            if (Provider == null || name.IsEmpty())
            {
                return;
            }

            Provider.Run(name);
        }

        /// <summary>
        /// Runs a dispatch with an array of args. Use this with caution
        /// as it creates an array with the args.
        /// </summary>
        public static void Run(string name, params object[] args)
        {
            if (Provider == null || name.IsEmpty())
            {
                return;
            }

            try
            {
                Provider.Run(name, args);
            }
            catch (Exception e)
            {
                Terminal.Log.Exception(e);
            }
        }

        /// <summary>
        /// Registers an instanced IObject to receive dispatch events.
        /// (Make sure to unregister the object when finished, or else
        /// it wont be collected by the GC)
        /// </summary>
        public static void Register<T>(T item) where T : class, IObject
        {
            Assert.IsNull(item);
            Provider.Register(item);
        }

        /// <summary>
        /// Unregisters an instanced IObject from the dispatch system.
        /// (Make sure to call this after you're finished with a registered
        /// object, or else the GC wont collect it)
        /// </summary>
        public static void Unregister<T>(T item) where T : class, IObject
        {
            Assert.IsNull(item);
            Provider.Unregister(item);
        }
    }
}
