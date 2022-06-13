using System;
using System.Collections;
using System.Collections.Generic;

namespace Eggshell
{
    /// <summary>
    /// A Simple components database container, that easily allows any class to have a set of 
    /// components, that can easily be accessed at anytime. Great for dependency injection
    /// </summary>
    public class Components<T> : IEnumerable<IComponent<T>> where T : class
    {
        public T Owner { get; }

        /// <summary>
        /// Creates an new components database. This should be done in your classes
        /// constructor, so there always is a database.
        /// </summary>
        public Components(T item)
        {
            Owner = item;
        }

        /// <summary>
        /// Creates an new components database with a inputted list, useful for
        /// when you want to have control over the list (say for networking). This
        /// should be done in your classes constructor, so there always is a database.
        /// </summary>
        public Components(T item, List<IComponent<T>> components) : this(item)
        {
            _storage = components;
        }

        /// <summary>
        /// Clears all the components from the registry when it is deconstructed, to
        /// make sure no logic gets fucked up (Might not? not sure, just in-case)
        /// </summary>
        ~Components()
        {
            Clear();
        }

        private List<IComponent<T>> _storage;
        protected List<IComponent<T>> Storage => _storage ??= new();

        // Enumerator
        // --------------------------------------------------------------------------------------- //

        public IEnumerator<IComponent<T>> GetEnumerator()
        {
            return _storage == null ? (IEnumerator<IComponent<T>>)Array.Empty<IComponent<T>>().GetEnumerator() : Storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // Controllers
        // --------------------------------------------------------------------------------------- //

        /// <summary>
        /// Adds a component to the components registry, checks if its a singleton
        /// component, if was replaces old with new.
        /// </summary>
        public virtual TComp Add<TComp>(TComp item) where TComp : class, IComponent<T>
        {
            Assert.IsNull(Owner);

            if (item == null)
            {
                Terminal.Log.Warning($"Trying to add a component that was null to {Owner}");
                return null;
            }

            if (item.Attached != null)
            {
                Terminal.Log.Error($"Component is already attached to something {Owner}");
                return null;
            }

            if (!item.Attachable(Owner))
            {
                return null;
            }

            // Replace if its a Singleton
            if (item is IObject lib && lib.ClassInfo.Components.Has<Singleton>() && TryGet(item.GetType(), out var comp))
            {
                Terminal.Log.Warning($"Replacing Component [{lib.ClassInfo.Name}]. Was Singleton");
                return Replace(comp, item);
            }

            item.Attached = Owner;

            Storage.Add(item);
            item.OnAttached();

            return item;
        }

        /// <summary>
        /// Checks if the components database already contains that component.
        /// returns true if it is in it.
        /// </summary>
        public bool Contains(IComponent<T> item)
        {
            return Storage.Contains(item);
        }

        /// <summary>
        /// Detaches and removes a component from the components database. 
        /// </summary>
        public void Remove(IComponent<T> item)
        {
            Detach(item);
            Storage.Remove(item);
        }

        /// <summary>
        /// Detaches and removes all components from the components database.
        /// Make sure to call this on deconstruction!
        /// </summary>
        public void Clear()
        {
            foreach ( var comp in Storage )
            {
                Detach(comp);
            }

            Storage.Clear();
        }

        /// <summary>
        /// Detaches a component from the registry / database. Used internally
        /// so users cant fuck up they shit.
        /// </summary>
        protected void Detach(IComponent<T> item)
        {
            item.OnDetached();
            item.Attached = null;

            if (Cached == item)
            {
                Cached = null;
            }
        }

        // Accessors API
        // --------------------------------------------------------------------------------------- //

        protected IComponent<T> Cached { get; set; }

        /// <summary>
        /// Gets the number of components that have been registered to this components
        /// database, use in conjunction with the indexer.
        /// </summary>
        public int Count => Storage.Count;

        /// <summary>
        /// A Simple array index accessor for the Components, so you can iterate
        /// over them without collecting garbage from IEnumerable.
        /// </summary>
        public IComponent<T> this[int key] => _storage[key];

        /// <summary>
        /// Gets a component from the database based off the inputted type of T.
        /// Caches the result so you can do prototype shit, without the performance
        /// cost.
        /// </summary>
        public virtual TComp Get<TComp>() where TComp : class
        {
            if (Cached is TComp cache)
            {
                return cache;
            }

            var index = 0;
            while (index <= Storage.Count - 1)
            {
                var item = Storage[index];
                if (item is TComp comp)
                {
                    Cached = item;
                    return comp;
                }

                index++;
            }

            return null;
        }

        /// <summary>
        /// Gets a component from the database based off the inputted type. Only checks
        /// if the types match, Not if its a subtype of the type
        /// </summary>
        public virtual IComponent<T> Get(Type type)
        {
            var index = 0;
            while (index <= Storage.Count - 1)
            {
                var item = Storage[index];

                if (item.GetType() == type)
                {
                    return item;
                }

                index++;
            }

            return null;
        }

        /// <summary>
        /// Creates and adds a component to the components registry. Great for quickly
        /// adding components that have a parameterless constructor
        /// </summary>
        public TComp Create<TComp>() where TComp : class, IComponent<T>, new()
        {
            return Add(new TComp());
        }

        /// <summary>
        /// Replaces a component with a new component, by removing the old form the database
        /// and then adding the new one.
        /// </summary>
        public TComp Replace<TComp>(IComponent<T> old, TComp newComp) where TComp : class, IComponent<T>
        {
            if (old == null || newComp == null)
            {
                Terminal.Log.Error("Components aren't valid");
                return null;
            }

            if (!Contains(old))
            {
                Terminal.Log.Error($"Components doesnt contain {old}");
                return null;
            }

            Remove(old);
            return Add(newComp);
        }

        /// <summary>
        /// Replaces a component with a new component, by removing the old form the database
        /// and then adding the new one.
        /// </summary>
        public TNew Replace<TOld, TNew>() where TNew : class, IComponent<T>, new() where TOld : class, IComponent<T>
        {
            var old = Get<TOld>();

            if (old == null)
            {
                Terminal.Log.Error("Components aren't valid");
                return null;
            }

            if (!Contains(old))
            {
                Terminal.Log.Error($"Components doesnt contain {old}");
                return null;
            }

            Remove(old);
            return Create<TNew>();
        }

        /// <summary>
        /// Trys to get the component of type T, and returns true if it could be found. Spews
        /// the out parameter with the found component, or else it is null
        /// </summary>
        public bool TryGet<TComp>(out TComp output) where TComp : class
        {
            output = Get<TComp>();
            return output != null;
        }

        /// <summary>
        /// Trys to get the component of type that is inputed, and returns true if it could
        /// be found. Spews the out parameter with the found component, or else it is null
        /// </summary>
        public bool TryGet(Type type, out IComponent<T> output)
        {
            output = Get(type);
            return output != null;
        }

        /// <summary>
        /// Checks to see if the database contains the inputted type of T. (Behind the scenes
        /// it gets the components, and returns if it was null or not)
        /// </summary>
        public bool Has<TComp>() where TComp : class, IComponent<T>
        {
            return Get<TComp>() != null;
        }

        /// <summary>
        /// Checks to see if the database contains the inputted type. (Behind the scenes
        /// it gets the components, and returns if it was null or not)
        /// </summary>
        public bool Has(Type type)
        {
            return Get(type) != null;
        }
    }
}
