using System;

namespace Eggshell.Diagnostics
{
    public interface IParameter
    {
        public Type Type { get; }
        public object Default { get; }
    }

    public struct Parameter : IParameter
    {
        public Type Type { get; }
        public object Default { get; }

        public Parameter(object value, Type type)
        {
            Type = type;
            Default = value;
        }

        public Parameter(object value) : this(value, value.GetType()) { }
    }

    public struct Parameter<T> : IParameter
    {
        public Type Type { get; }
        public object Default { get; }

        public Parameter(T value)
        {
            Type = typeof(T);
            Default = value;
        }
    }
}
