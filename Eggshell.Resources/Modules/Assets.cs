using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Eggshell.IO;

namespace Eggshell.Resources
{
    /// <summary>
    /// The assets module is responsible for handling the loading and unloading
    /// of resources and assets. This API will help you greatly when trying to
    /// make data loaded at runtime and compiled at runtime.
    /// </summary>
    public sealed class Assets : Module
    {
        public static Assets Registry
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Get<Assets>();
        }

        // Resources API
        // --------------------------------------------------------------------------------------- //

        public static Handle<T> Load<T>(int hash, bool persistant = false) where T : class, IAsset, new()
        {
            return Find<T>(hash).Load(persistant) ?? Fallback<T>();
        }

        public static Resource<T> Find<T>(int hash) where T : class, IAsset, new()
        {
            return Registry[hash] as Resource<T>;
        }

        public static Handle<T> Load<T>(Pathing path, bool persistant = false) where T : class, IAsset, new()
        {
            return Find<T>(path)?.Load(persistant) ?? Fallback<T>();
        }

        public static Resource<T> Find<T>(Pathing path) where T : class, IAsset, new()
        {
            Library library = typeof(T);

            // Apply shorthand, if path doesn't have one
            if (!path.IsValid() && library.Components.TryGet<Pathable>(out var attribute))
            {
                path = $"{attribute.Short}://" + path;
            }

            path = path.Virtual().Normalise();

            if (Registry[path] is Resource<T> resource)
            {
                return resource;
            }

            return !path.IsFile() ? null : Registry.Fill<T>(path);
        }

        public static Handle<T> Fallback<T>() where T : class, IAsset, new()
        {
            Library library = typeof(T);

            // Load default resource, if its not there
            if (!library.Components.TryGet(out Archive files) || files.Fallback.IsEmpty())
            {
                return null;
            }

            Terminal.Log.Error($"Loading fallback for [{library.Title}]");

            var fallback = files.Fallback;
            fallback = fallback.Virtual().Normalise();

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
            path = path.Absolute().Virtual().Normalise();

            var instance = new Resource<T>(path.Hash(), path.Extension(), () => path.Absolute().Info<FileInfo>().OpenRead());
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
