using System;
using System.Runtime.CompilerServices;

namespace Eggshell.Resources
{
    public readonly struct HandleAwaiter<T> : INotifyCompletion where T : class, IAsset
    {
        private readonly IHandle<T> _handle;

        public HandleAwaiter(IHandle<T> handle)
        {
            _handle = handle;
        }

        public bool IsCompleted => _handle.IsLoaded;

        public void OnCompleted(Action continuation)
        {
            _handle.Request(_ => continuation?.Invoke());
        }

        public T GetResult()
        {
            return _handle.Cached();
        }
    }
}
