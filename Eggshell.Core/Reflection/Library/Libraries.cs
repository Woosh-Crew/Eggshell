using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Eggshell
{
    /// <summary>
    /// Libraries is used for handling every library instance. It is
    /// also responsible for caching libraries too. Its in its own
    /// class so we can decouple it, plus provide a wrapper / our own
    /// accessors to the database.
    /// </summary>
    [Serializable, Group("Reflection")]
    public class Libraries : IObject, IEnumerable<Library>
    {
        private readonly SortedList<int, Library> _storage = new();
        public Library ClassInfo => typeof(Libraries);

        // Accessors
        // --------------------------------------------------------------------------------------- //

        /// <summary>
        /// Finds a library by its deterministic Id, which is generated
        /// at compile time, from the classroom
        /// </summary>
        public Library this[int hash]
        {
            get
            {
                try
                {
                    return _storage[hash];
                }
                catch (KeyNotFoundException)
                {
                    Terminal.Log.Error($"Classname ID [{hash}], was not found in Library Database");
                    return null;
                }
            }
        }

        /// <summary>
        /// Finds a library by its name, which that string gets converted
        /// converted to its determinsitic int id (hashed string)
        /// </summary>
        public Library this[string key]
        {
            get
            {
                try
                {
                    return _storage[key.Hash()];
                }
                catch (KeyNotFoundException)
                {
                    Terminal.Log.Error($"Classname {key}, was not found in Library Database");
                    return null;
                }
            }
        }

        /// <summary>
        /// Finds a library by its type, where it just does a linq operation to
        /// find the first type that equals the one in the library
        /// </summary>
        public Library this[Type key] => _storage.Values.FirstOrDefault(e => e.Info == key);

        /// <summary>
        /// This will find the library that holds this type. Or will
        /// find a type that is implementing an interface from the inputted
        /// type, (as confusing as that sounds). This is very helpful
        /// for finding types easily. As it handles interface logic
        /// </summary>
        public Library Find(Type type)
        {
            if (type.IsAbstract || type.IsInterface)
            {
                return this.FirstOrDefault(e => (type.IsInterface ? e.Info.HasInterface(type) : e.Info.IsSubclassOf(type)) && !e.Info.IsAbstract);
            }

            var potential = this.FirstOrDefault(e => e.Info != type && (type.IsInterface ? e.Info.HasInterface(type) : e.Info.IsSubclassOf(type)) && !e.Info.IsAbstract);
            return potential ?? type;
        }

        /// <summary>
        /// This will find the library that holds this type with an added
        /// search pattern, where you can cull results based on components
        /// or what ever. Or will find a type that is implementing an interface
        /// from the inputted type, (as confusing as that sounds).
        /// This is very helpful for finding types easily.
        /// As it handles interface logic
        /// </summary>
        public Library Find(Type type, Func<Library, bool> search)
        {
            if (type.IsAbstract || type.IsInterface)
            {
                return this.FirstOrDefault(e => (type.IsInterface ? e.Info.HasInterface(type) : e.Info.IsSubclassOf(type)) && !e.Info.IsAbstract && search.Invoke(e));
            }

            var potential = this.FirstOrDefault(e => e.Info != type && (type.IsInterface ? e.Info.HasInterface(type) : e.Info.IsSubclassOf(type)) && !e.Info.IsAbstract && search.Invoke(e));
            return potential ?? type;
        }

        /// <inheritdoc cref="Find(Type)"/>
        public Library<T> Find<T>() where T : class, IObject
        {
            return (Library<T>)Find(typeof(T));
        }

        /// <inheritdoc cref="Find(Type, Func{Library, bool})"/>
        public Library Find<T>(Func<Library, bool> search) where T : class
        {
            return Find(typeof(T), search);
        }

        /// <summary>
        /// Gets all Libraries with the given components
        /// </summary>
        public IEnumerable<Library> With<T>() where T : IComponent<Library>
        {
            return this.Where(e => e.Components.Has<T>());
        }

        /// <summary>
        /// This will get all libraries where they are a subclass of
        /// the type T, or will get types that implement the inputted
        /// interface. 
        /// </summary>
        public IEnumerable<Library> All<T>() where T : class
        {
            var type = typeof(T);
            return type.IsInterface ? this.Where(e => e.Info.HasInterface<T>()) : this.Where(e => e.Info.IsSubclassOf(type));
        }

        // API

        public void Add(Library item)
        {
            var hashedName = item.Name.Hash();

            // Store it in Database
            if (_storage.ContainsKey(hashedName))
            {
                Terminal.Log.Warning($"Replacing Library [{item.Name}]");
                _storage[hashedName] = item;
                return;
            }

            _storage.Add(hashedName, item);
        }

        public void Add(Type type)
        {
            if (!type.IsDefined(typeof(LibraryAttribute), false))
            {
                Add(new Library(type.Name.ToProgrammerCase(), type));
                return;
            }

            // If we have meta present, use it
            var attribute = type.GetCustomAttribute<LibraryAttribute>();
            Add(new Library(attribute!.Name, type));
        }

        public void Add(Assembly assembly, bool precompiled = true)
        {
            if (precompiled)
            {
                assembly.GetType("Eggshell.Generated.Classroom")?.GetMethod("Cache", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, null);
                return;
            }

            foreach (var type in assembly.GetTypes())
            {
                if (Library.IsValid(type))
                {
                    Add(type);
                }
            }
        }

        public void Add(AppDomain domain)
        {
            var main = typeof(Library).Assembly;
            Add(main);

            foreach (var assembly in domain.GetAssemblies())
            {
                if (assembly != main)
                {
                    Add(assembly);
                }
            }
        }

        // Enumerator

        public IEnumerator<Library> GetEnumerator()
        {
            return _storage.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
