using System;

namespace Eggshell.Reflection
{
    public delegate T Getter<out T>(object target);

    public delegate void Setter<in T>(object target, T value);

    [Skip]
    public class DynamicProperty<T> : Property<T>
    {
        public DynamicProperty(string name) : base(name, null) { }

        public Getter<T> Getter { get; set; }
        public Setter<T> Setter { get; set; }

        protected override void Get(object from, out T value)
        {
            value = Getter == null ? default : Getter.Invoke(from);
        }

        protected override void Set(object target, T value)
        {
            Setter?.Invoke(target, value);
        }
    }

    [Skip]
    public class NativeProperty<T> : Property<T>
    {
        public NativeProperty(string name) : base(name, null) { }

        protected override void Get(object from, out T value)
        {
            throw new NotImplementedException();
        }

        protected override void Set(object from, T value)
        {
            throw new NotImplementedException();
        }
    }

    [Skip]
    public class StaticProperty<T> : Property<T>
    {
        public Action<T> OnChanged { get; set; }

        public override T Value
        {
            get => _value;
            set
            {
                _value = value;
                OnChanged?.Invoke(value);
            }
        }

        public StaticProperty(string name) : base(name, null) { }

        // Property

        private T _value;

        protected override void Get(object from, out T value)
        {
            value = Value;
        }

        protected override void Set(object from, T value)
        {
            Value = value;
        }
    }
}
