using System;
using System.Diagnostics;

namespace Eggshell
{
    public partial class Library
    {
        /// <summary>
        /// Database for Library Records. Allows the access of all records.
        /// Use extension methods to add functionality to database access.
        /// </summary>
        public static Libraries Database { get; private set; }

        static Library()
        {
            Database = new();

            var stopwatch = Stopwatch.StartNew();

            Database.Add(AppDomain.CurrentDomain);

            stopwatch.Stop();
            Terminal.Log.Info($"Library Ready | {stopwatch.Elapsed.TotalMilliseconds} ms");
        }

        internal static bool IsValid(Type type)
        {
            return type.HasInterface<IObject>() || type.IsDefined(typeof(LibraryAttribute), true);
        }

        /// <summary>
        /// Registers the target object with the Library.
        /// Which allows it to receive instance callbacks and
        /// returns its library instance.
        /// </summary>
        public static Library Register(IObject value)
        {
            Library lib = value.GetType();
            Assert.IsNull(lib);

            return lib.OnRegister(value) ? lib : null;
        }

        /// <summary>
        /// Cleans up IObject object, removes it from instance
        /// callback database so the garbage collector picks it up.
        /// </summary>
        public static void Unregister(IObject value)
        {
            value.ClassInfo.OnUnregister(value);
        }

        /// <summary> <inheritdoc cref="Create"/> and returns T </summary>
        public static T Create<T>(Library lib = null)
        {
            lib ??= typeof(T);
            return (T)lib.Create();
        }

        public static implicit operator Library(string value)
        {
            return Database[value];
        }

        public static implicit operator Library(Type value)
        {
            return Database[value];
        }

        public static implicit operator Library(int hash)
        {
            return Database[hash];
        }
    }
}
