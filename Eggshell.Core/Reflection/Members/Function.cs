using System;
using System.Collections.Generic;
using System.Reflection;

namespace Eggshell.Reflection
{
    /// <summary>
    /// Properties are used in libraries for defining variables
    /// that can be inspected and changed.
    /// </summary>
    [Serializable, Group("Reflection")]
    public class Function : IObject, IMember<MethodInfo>
    {
        /// <summary> 
        /// The MethodInfo that this function was generated for. 
        /// caching its meta data in the constructor.
        /// </summary>
        public MethodInfo Info => Origin == null ? null : _info ??= Parent.Info.GetMethod(Origin, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        /// <summary>
        /// The cached method info that is generated from
        /// <see cref="Function.Info"/>.
        /// </summary>
        private MethodInfo _info;

        /// <summary>
        /// Where did this function come from? (This will automatically
        /// pull function from this Library) - (Data is filled
        /// out by source generators)
        /// </summary>
        public Library Parent { get; set; }

        /// <summary>
        /// Function's IObject implementation for Library. as ironic
        /// as that sounds. Its used for getting meta about the Property
        /// </summary>
        public Library ClassInfo => typeof(Function);

        /// <summary>
        /// It isn't recommended that you create a function manually, as
        /// this is usually done through source generators.
        /// </summary>
        public Function(string name, string origin)
        {
            Name = name;
            Origin = origin;

            Id = Name.Hash();
        }

        /// <summary>
        /// The name of the function member that this eggshell property
        /// was generated from. Used when getting the property itself. 
        /// </summary>
        public string Origin { get; }

        /// <summary>
        /// The programmer friendly name of this function, that is used
        /// to generate a deterministic hash for the ID.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The deterministic id created from <see cref="Property{T}.Name"/>,
        /// which is used in a Binary Tree / Sorted List to get functions
        /// by name.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// The nice looking name for this function. Used in UI as well as
        /// response from the user because they don't like looking at weird names
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The help message that tells users about this function. This is usually
        /// generated from the XML documentation above a function.
        /// </summary>
        public string Help { get; set; }

        /// <summary>
        /// The group that this function belongs to. This is used for getting
        /// all functions by group, as well as other functionality depending
        /// on the class that this function is from.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Does this function require an instance accessor to be invoked?
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// Gets the default arguments for a function parameter list,
        /// will use the inputted (in order of params) or the default
        /// value from the parameter.
        /// </summary>
        protected object[] GetDefaultArgs(IReadOnlyList<object> input)
        {
            var parameters = Info.GetParameters();

            if (parameters.Length == 0)
            {
                return null;
            }

            var args = new object[parameters.Length];

            for (var i = 0; i < args.Length; i++)
            {
                args[i] = input != null && i < input.Count ? input[i] : parameters[i].DefaultValue;
            }

            return args;
        }

        /// <summary>
        /// Invokes this function
        /// </summary>
        public object Invoke(IObject target)
        {
            return Invoke(IsStatic ? null : target, GetDefaultArgs(null));
        }

        /// <summary>
        /// Invokes this function with a return type of T
        /// </summary>
        public T Invoke<T>(IObject target)
        {
            return (T)Invoke(IsStatic ? null : target, GetDefaultArgs(null));
        }

        /// <summary>
        /// Invokes this function with an array of parameters
        /// </summary>
        public virtual object Invoke(IObject target, params object[] parameters)
        {
            return Info.Invoke(IsStatic ? null : target, GetDefaultArgs(parameters));
        }

        /// <summary>
        /// Invokes this function with an array of parameters and returns the
        /// type of T
        /// </summary>
        public T Invoke<T>(IObject target, params object[] parameters)
        {
            return (T)Invoke(IsStatic ? null : target, GetDefaultArgs(parameters));
        }
    }
}
