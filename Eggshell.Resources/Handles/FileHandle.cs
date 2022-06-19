using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Eggshell.Resources
{
    internal class FileHandle<T> : IHandle<T> where T : class, IAsset
    {
        private FileInfo File { get; }

        public FileHandle(FileInfo file)
        {
            File = file;
        }

        public bool IsLoaded => _isLoaded;

        public T Asset
        {
            set => _asset = value;
        }

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

            _stream = File.OpenRead();
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
        internal void Unload(Action callback = null)
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
        public void Request(Action<T> callback = null)
        {
            if (!Valid())
            {
                return;
            }

            if (IsLoaded)
            {
                callback?.Invoke(_asset);
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
    }
}
