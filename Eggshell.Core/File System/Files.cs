using System;
using System.Diagnostics;
using System.IO;
using Eggshell.IO;

namespace Eggshell
{
    /// <summary>
    /// Files, is Espionage.Engines File System.
    /// All Saving, Loading, ETC. You can use
    /// short hands for defining paths.
    /// </summary>
    [Link, Group("Files"), Title("File System")]
    public static class Files
    {
        public static Serializer Serialization { get; } = new();

        //
        // API
        //
        public static Pathing Pathing(string path)
        {
            return new(path);
        }

        public static Pathing Pathing(FileInfo path)
        {
            return new(path.FullName);
        }

        /// <inheritdoc cref="Save{T}(T,string)"/>
        public static void Save<T>(Library lib, T item, string path)
        {
            Serialization.Store(Serialization.Serialize(lib, item), path);
        }

        /// <summary>
        /// Saves anything you want, (provided theres a
        /// serializer for it) to the given path
        /// </summary>
        public static void Save<T>(T item, string path)
        {
            Serialization.Store(Serialization.Serialize(item), path);
        }

        /// <inheritdoc cref="Save{T}(T,string)"/>
        public static void Save<T>(Library lib, T[] item, string path)
        {
            Serialization.Store(Serialization.Serialize(lib, item), path);
        }

        /// <summary>
        /// Saves an array of anything you want,
        /// (provided theres a serializer for it)
        /// to the given path
        /// </summary>
        public static void Save<T>(T[] item, string path)
        {
            Serialization.Store(Serialization.Serialize(item), path);
        }

        /// <summary>
        /// Deletes the file at the given path
        /// </summary>
        public static void Delete(Pathing path)
        {
            path.Absolute();

            if (path.Exists())
            {
                File.Delete(path);
            }

            Terminal.Log.Error($"File [{path}], couldn't be deleted.");
        }

        /// <summary>
        /// Deletes all files with the given extension at the path
        /// </summary>
        public static void Delete(Pathing path, string extension)
        {
            foreach (var item in path.Absolute().All(SearchOption.TopDirectoryOnly, $"*.{extension}"))
            {
                File.Delete(item);
            }
        }

        /// <inheritdoc cref="Delete(IO.Pathing, string)"/> 
        public static void Delete(Pathing path, params string[] extension)
        {
            if (!path.Exists())
            {
                Terminal.Log.Error($"Path [{path}], doesn't exist");
                return;
            }

            foreach (var item in extension)
            {
                Delete(path, item);
            }
        }


        /// <summary>
        /// Copies the source file or directory to the target path
        /// </summary>
        public static void Copy(Pathing sourcePath, Pathing targetPath, bool overwrite = true)
        {
            sourcePath.Absolute();
            targetPath.Absolute();

            // Is File
            if (sourcePath.Meta().IsFile)
            {
                var fileInfo = new FileInfo(sourcePath);

                if (!File.Exists(sourcePath))
                {
                    throw new FileNotFoundException();
                }

                targetPath.Create();
                fileInfo.CopyTo(targetPath + $"{sourcePath.Name()}", overwrite);
            }
            // Is Directory
            else
            {
                foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
                }

                foreach (var newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
                }
            }
        }

        /// <summary>
        /// Moves the source file to the target destination
        /// </summary>
        public static void Move(Pathing source, Pathing destination)
        {
            source.Absolute();
            destination.Absolute();

            if (!source.Exists())
            {
                throw new FileNotFoundException();
            }

            File.Move(source, destination);
        }

        /// <summary>
        /// Opens the given directory in the OS's File Explorer,
        /// or opens the given file in the default application
        /// </summary>
        public static void Open(Pathing path)
        {
            path.Absolute();

            if (!path.Exists())
            {
                Terminal.Log.Warning($"Path or File [{path}], doesn't exist");
                return;
            }

            Process.Start($"file://{path}");
        }

        //
        // Structs
        //

        public readonly struct Meta
        {
            internal Meta(FileAttributes attributes, DateTime creation, DateTime access, DateTime modified)
            {
                Attributes = attributes;
                Creation = creation;
                Access = access;
                Modified = modified;
            }

            public FileAttributes Attributes { get; }

            // Helpers

            public bool IsFile => Attributes is not FileAttributes.Directory;
            public bool IsDirectory => Attributes is FileAttributes.Directory;

            // Timings

            public DateTime Creation { get; }
            public DateTime Access { get; }
            public DateTime Modified { get; }
        }
    }
}
