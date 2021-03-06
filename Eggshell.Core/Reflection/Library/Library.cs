using System;
using System.Collections.Generic;
using Eggshell.Reflection;

namespace Eggshell
{
    /// <summary>
    /// Libraries are used for storing meta data on a type, this includes
    /// Title, Name, Group, Icons, etc. You can also add your own data
    /// using components. We can do a lotta cool and performant
    /// shit because of this. Such as easily doing C# / C++ calls.
    /// </summary>
    [Serializable, Group("Reflection")]
    public partial class Library : IObject, IMeta
    {
        /// <summary>
        /// The type this library was generated for, caching
        /// its meta data in the constructor. 
        /// </summary>
        public Type Info { get; }

        /// <summary>
        /// What is this library inherited from. (This will automatically
        /// pull properties and methods from this Library) - (Data is filled
        /// out by source generators)
        /// </summary>
        public Library Parent { get; }

        /// <summary>
        /// What is using this Library for inheritance. (Data is filled
        /// out by source generators)
        /// </summary>
        public List<Library> Children { get; } = new();

        /// <summary>
        /// Library's IObject implementation for Library. as ironic
        /// as that sounds. Its used for getting meta about Library
        /// </summary>
        public Library ClassInfo => typeof(Library);

        /// <summary>
        /// Components are added meta data onto that library, this can
        /// include icons, company, stylesheet, etc. They allow us
        /// to do some really crazy cool shit
        /// </summary>
        public Components<Library> Components { get; }

        /// <summary>
        /// Properties are variables that the library owns. This is
        /// usually mutable value types if the property has a setter
        /// </summary>
        [Skip] public Members<Property> Properties { get; }

        /// <summary>
        /// Functions are tasks that the library does broken up into
        /// groups so its easier to digest how a program works.
        /// </summary>
        [Skip] public Members<Function> Functions { get; }

        /// <summary>
        /// It isn't recommended that you create the library manually, as
        /// this is usually done through source generators.
        /// </summary>
        public Library(string name, int id, Type type, Library parent = null)
        {
            Assert.IsNull(type);

            Info = type;

            Properties = new(this);
            Functions = new(this);

            Name = name;
            Id = id;

            Parent = parent;

            if (Parent != null)
            {
                Parent.Children.Add(this);

                // Inherit Members
                Properties.Add(Parent.Properties);
                Functions.Add(Parent.Functions);
            }

            Components = new(this);
        }

        /// <summary>
        /// IBinding is a binding to a library. Allows you to inject logic
        /// in to the library pipeline, as well as store custom library meta
        /// data for use in your project. (Such as the Archive Component)
        /// </summary>
        public interface Binding : IComponent<Library>
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

        /// <summary>
        /// It isn't recommended that you create the library manually, as
        /// this is usually done through source generators. This will automatically
        /// generate an id for the library, based off the name (hashed) 
        /// </summary>
        public Library(string name, Type type, Library parent = null) : this(name, name.Hash(), type, parent) { }

        /// <summary>
        /// Creates an IObject, this just does some sanity checking before
        /// calling the internal Construct() method, which can be overridden
        /// or uses a constructor attribute.
        /// </summary>
        public virtual IObject Create()
        {
            // This gets source generated, to be compile time efficient

            for (var index = 0; index < Components.Count; index++)
            {
                var instance = (Components[index] as Binding)?.OnCreate();

                if (instance != null)
                {
                    return instance;
                }
            }

            return Construct();
        }

        /// <summary>
        /// Creates a type of T, this just does some sanity checking before
        /// calling the internal Construct() method, which can be overridden
        /// or uses a constructor attribute.
        /// </summary>
        public T Create<T>() where T : IObject
        {
            return (T)Create();
        }

        /// <summary>
        /// Constructs this IObject, can be overridden to provide custom logic.
        /// such as using a custom constructor, or setting off events when
        /// this library has been constructed. Use wisely!
        /// </summary>
        protected virtual IObject Construct()
        {
            // This gets source generated, to be compile time efficient

            if (!Info.IsAbstract)
            {
                return (IObject)Activator.CreateInstance(Info);
            }

            Terminal.Log.Error($"Can't construct {Name}, is abstract and doesn't have constructor predefined.");
            return null;
        }

        /// <summary>
        /// A Callback for when an object is created and registered
        /// to this library. Incredibly useful for setting up instanced
        /// based callbacks, as well as keeping track of instances.
        /// </summary>
        protected virtual bool OnRegister(IObject value)
        {
            // This gets source generated, to be compile time efficient

            for (var index = 0; index < Components.Count; index++)
            {
                var potential = (Components[index] as Binding)?.OnRegister(value) ?? true;

                if (!potential)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// A Callback for when an object is trying to be unregistered
        /// to this library. Incredibly useful for setting up instanced
        /// based callbacks, as well as keeping track of instances.
        /// </summary>
        protected virtual void OnUnregister(IObject value)
        {
            // This gets source generated, to be compile time efficient

            for (var index = 0; index < Components.Count; index++)
            {
                (Components[index] as Binding)?.OnUnregister(value);
            }
        }

        /// <summary>
        /// Checks to see if this library inherits from T, useful for making
        /// sure you are casting to the right type.
        /// </summary>
        public virtual bool Inherits<T>()
        {
            var type = typeof(T);
            return type.IsInterface ? Info.HasInterface(type) : Info.IsSubclassOf(type);
        }

        /// <summary>
        /// Checks to see if this library inherits from T, useful for making
        /// sure you are casting to the right type.
        /// </summary>
        public virtual bool Inherits(Type type)
        {
            return type.IsInterface ? Info.HasInterface(type) : Info.IsSubclassOf(type);
        }

        /// <summary>
        /// The programmer friendly name of this type, that is used
        /// to generate a deterministic hash for the ID.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The deterministic id created from <see cref="Library.Name"/>,
        /// which is used in a Binary Tree / Sorted List to get the library
        /// by name, and also is used for type checking for compiled data.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// The nice looking name for this library. Used in UI as well as
        /// response from the user if they don't like looking at weird names
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The group that this library belongs to. This is used for getting
        /// all libraries, as well as other functionality depending on the class
        /// that is inherited from a specific type (like asset)
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// The help message that tells users about this library. This is usually
        /// generated from the XML documentation above a type. Make sure your classes
        /// are documented so they are readable by both programmers and people who use
        /// your application!
        /// </summary>
        public string Help { get; set; }
    }
}
