using System;

namespace Eggshell.Resources
{
    /// <summary>
    /// A Handle is responsible for controlling how an asset is loaded,
    /// which is invoked by the resource. We do this so we can async load
    /// assets, or add your own extension methods to the loading process.
    /// </summary>
    public interface IHandle<T> where T : class, IAsset
    {
        bool IsLoaded { get; }
        T Asset { set; }

        /// <summary>
        /// Returns the cached asset if it is valid and was already loaded,
        /// returns null if it fucked up. You shouldn't need to use this.
        /// </summary>
        T Cached();

        /// <summary>
        /// Sets up a request by registering a callback for when the asset
        /// is loaded.
        /// </summary>
        void Request(Action<T> callback = null);

        /// <summary>
        /// A custom awaiter handle so you can do "await Resource.Load()"
        /// very powerful api I must say...
        /// </summary>
        HandleAwaiter<T> GetAwaiter()
        {
            return new(this);
        }
    }
}
