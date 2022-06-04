using System.Collections.Generic;
using Eggshell.Dispatching;
using Eggshell.Reflection;

namespace Eggshell
{
    public class Dispatcher : IDispatcher
    {
        public Library ClassInfo { get; }

        public Dispatcher()
        {
            ClassInfo = Library.Register(this);
        }

        private Dictionary<string, List<Function>> _events = new();
        private Dictionary<Library, List<IObject>> _registry = new();

        public void Add(string name, Function function)
        {
            if (!_events.ContainsKey(name))
            {
                _events.Add(name, new());
            }

            _events[name]?.Add(function);
        }

        public void Run(string name)
        {
            Run(name, null);
        }

        public void Run(string name, params object[] args)
        {
            if (!_events.TryGetValue(name, out var callbacks))
            {
                return;
            }

            foreach (var function in callbacks)
            {
                if (function.IsStatic)
                {
                    function.Invoke(null, args);
                    continue;
                }

                foreach (var item in _registry[function.Parent])
                {
                    function.Invoke(item, args);
                }
            }
        }

        public void Register(IObject item)
        {
            var type = item.GetType();

            if (!_registry.ContainsKey(type))
            {
                _registry.Add(type, new());
            }

            if (_registry.TryGetValue(type, out var all))
            {
                all.Add(item);
            }
        }

        public void Unregister(IObject item)
        {
            if (_registry.TryGetValue(item.GetType(), out var all))
            {
                all.Remove(item);
            }
        }
    }
}
