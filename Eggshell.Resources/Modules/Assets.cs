using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Eggshell.IO;

namespace Eggshell.Resources
{
    /// <summary>
    /// The assets module is responsible for handling the loading and unloading
    /// of resources and assets using a high level API. This API will help you
    /// greatly when trying to load custom data at runtime and compiled at runtime.
    /// </summary>
    public sealed class Assets : Module
    {
        public static Assets Registry
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Get<Assets>();
        }

        /// <summary>
        /// Returns a handle from a resource (Basically calls the resource load,
        /// or fallbacks one) from the hash of a resource. 
        /// </summary>
        public static IHandle<T> Load<T>(int hash, bool persistant = false) where T : class, IAsset, new()
        {
            return Find<T>(hash).Load(persistant) ?? Fallback<T>();
        }

        /// <summary>
        /// Trys to find the correct resource based off the inputted T input.
        /// and returns it, null if none was found.
        /// </summary>
        public static Resource<T> Find<T>(int hash) where T : class, IAsset, new()
        {
            if (Registry[hash] is Resource<T> resource)
            {
                return resource;
            }

            Terminal.Log.Error($"Couldn't find resource with ID [{hash}]");
            return null;

        }

        // Pathing

        /// <summary>
        /// Returns a handle from a resource (Basically calls the resource load,
        /// or fallbacks one) from the inputted path. Which virtualizes it and
        /// normalizes it. 
        /// </summary>
        public static IHandle<T> Load<T>(Pathing path, bool persistant = false) where T : class, IAsset, new()
        {
            return Find<T>(path)?.Load(persistant) ?? Fallback<T>();
        }

        /// <summary>
        /// Trys to find the correct resource based off the inputted T input and
        /// the inputted path. Fills the path if none was found.
        /// </summary>
        public static Resource<T> Find<T>(Pathing path) where T : class, IAsset, new()
        {
            Library library = typeof(T);

            // Apply shorthand, if path doesn't have one
            if (!path.IsValid() && library.Components.TryGet(out Pathable pathable))
            {
                path = $"{pathable.Short}://" + path;
            }

            path = path.Virtual().Normalise();

            if (Registry[path] is Resource<T> resource)
            {
                return resource;
            }

            if (path.IsFile())
            {
                return Registry.Fill<T>(path);
            }

            Terminal.Log.Error($"No file found at path [{path}]");
            return null;
        }

        // URI

        public static IHandle<T> Load<T>(Uri uri, bool persistant = false) where T : class, IAsset, new()
        {
            return Find<T>(uri)?.Load(persistant) ?? Fallback<T>();
        }

        public static Resource<T> Find<T>(Uri uri) where T : class, IAsset, new()
        {
            Terminal.Log.Error("URI's are not supported yet!");
            return null;
        }

        // Default

        public static IHandle<T> Fallback<T>() where T : class, IAsset, new()
        {
            Library library = typeof(T);

            // Load default resource, if its not there
            if (!library.Components.TryGet(out Archive files) || files.Fallback.IsEmpty())
            {
                return null;
            }

            Terminal.Log.Error($"Loading fallback for [{library.Title}]");

            var fallback = files.Fallback.Virtual().Normalise();
            return fallback.IsFile() ? Load<T>(fallback, true) : null;
        }

        // Registry
        // --------------------------------------------------------------------------------------- //

        public Resource this[int key] => _storage.TryGetValue(key, out var resource) ? resource : null;
        public Resource this[Pathing path] => _storage.TryGetValue(path.Virtual().Normalise().Hash(), out var resource) ? resource : null;

        public void Fill<T>(Resource<T> resource) where T : class, IAsset, new()
        {
            _storage.Add(resource.Identifier, resource);
        }

        public Resource<T> Fill<T>(Pathing path) where T : class, IAsset, new()
        {
            if (!path.Exists() || !path.IsFile())
            {
                Terminal.Log.Error("Invalid Resource Fill from Pathing");
                return null;
            }

            path = path.Absolute().Virtual().Normalise();

            var instance = new Resource<T>(new FileHandle<T>(path.Absolute().Info<FileInfo>()), path.Hash(), path.Extension());
            instance.Components.Add(new Origin() { Path = path });

            _storage.Add(instance.Identifier, instance);

            return instance;
        }

        // Internal Logic
        // --------------------------------------------------------------------------------------- //

        private readonly SortedList<int, Resource> _storage = new();

        protected override void OnShutdown()
        {
            foreach ( var resource in _storage.Values )
            {
                // Unload all resources, forcefully.
                resource.Unload(null, true);
            }

            _storage.Clear();
        }
    }
}
