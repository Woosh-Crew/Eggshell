using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Eggshell.Resources
{
    public readonly struct HandleAwaiter<T> : INotifyCompletion where T : class, IAsset
    {
        private readonly Handle<T> _handle;

        public HandleAwaiter(Handle<T> handle)
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

    /// <summary>
    /// A Handle is responsible for controlling how an asset is loaded,
    /// which is invoked by the resource. We do this so we can async load
    /// assets, or add your own extension methods to the loading process.
    /// </summary>
    public class Handle<T> : IRoutine where T : class, IAsset
    {
        private Func<Stream> Stream { get; }

        public Handle(Func<Stream> stream)
        {
            Stream = stream;
        }

        internal void Setup(Func<T> asset)
        {
            _asset ??= asset.Invoke();
        }

        public bool IsLoaded => _isLoaded;

        // Request

        private IDisposable _stopwatch;
        private Stream _stream;

        private T _asset;

        private bool _isLoaded;
        private bool _loading;

        // Loading
        // --------------------------------------------------------------------------------------- //

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Load()
        {
            if (IsLoaded || _loading)
            {
                // Nothing was loaded
                return;
            }

            _stopwatch = Terminal.Stopwatch($"Asset Loaded [{_asset}]");

            _stream = Stream.Invoke();
            _loading = true;

            _asset.Load(_stream, OnLoad);
        }

        private Action<T> _onLoad;

        private void OnLoad()
        {
            _isLoaded = true;
            _loading = false;

            _stopwatch?.Dispose();
            _stopwatch = null;

            _stream?.Dispose();
            
            _onLoad?.Invoke(_asset);
            _onLoad = null;
        }

        // Unloading
        // --------------------------------------------------------------------------------------- //

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Unload(Action callback = null)
        {
            if (!IsLoaded || _loading)
            {
                // Nothing was loaded
                return;
            }

            _onUnload = callback;

            using var _ = Terminal.Stopwatch($"Asset Unloaded [{_asset}]");

            _asset.Unload(OnUnload);
            _asset.Delete();
        }

        private Action _onUnload;

        private void OnUnload()
        {
            _isLoaded = false;
            _loading = false;

            _stopwatch.Dispose();
            _stopwatch = null;

            _onUnload?.Invoke();
            _onUnload = null;

            _asset = null;
        }

        // Public API
        // --------------------------------------------------------------------------------------- //

        /// <summary>
        /// A custom awaiter handle so you can do "await Resource.Load()"
        /// very powerful api I must say...
        /// </summary>
        public HandleAwaiter<T> GetAwaiter()
        {
            return new(this);
        }

        /// <summary>
        /// Returns the cached asset if it is valid and was already loaded,
        /// returns null if it fucked up. You shouldn't need to use this.
        /// </summary>
        public T Cached()
        {
            return Valid() && IsLoaded ? _asset : null;
        }

        /// <summary>
        /// Sets up a request by registering a callback for when the asset
        /// is loaded.
        /// </summary>
        public void Request(Action<T> callback)
        {
            if (!Valid())
            {
                return;
            }

            if (IsLoaded)
            {
                callback.Invoke(_asset);
                return;
            }

            _onLoad += callback;
            Load();
        }

        /// <summary>
        /// A sanity checker for checking if the asset we're trying to load
        /// actually exists. Returns true if it does.
        /// </summary>
        public bool Valid()
        {
            if (_asset != null)
                return true;

            Terminal.Log.Error("Asset was invalid.");
            return false;
        }

        // IRoutine
        // --------------------------------------------------------------------------------------- //

        float IRoutine.Progress => 0;
        string IRoutine.Text => "Loading";

        void IRoutine.Load(Action loaded)
        {
            Request(_ => loaded.Invoke());
        }
    }
}
