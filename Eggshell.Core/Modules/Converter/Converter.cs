using System;
using Eggshell.Converters;

namespace Eggshell
{
    /// <summary>
    /// Eggshell's string to object converter. Converts objects by looking for a valid
    /// converter using the reflection system, creates it and calls its convert method.
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// Converts a string to the type of T. Uses eggshells reflection system to find
        /// the correct converter, and uses it to convert the string to T.
        /// </summary>
        public static T Convert<T>(this string value)
        {
            if (typeof(T).IsEnum)
            {
                return (T)Enum.Parse(typeof(T), value);
            }

            var converter = Library.Database.Find<IConverter<T>>()?.Create<IConverter<T>>();
            Assert.IsNull(converter, $"No Valid converters for {typeof(T).Name}.");

            return converter!.Convert(value);
        }

        /// <summary>
        /// Trys to convert a string to the type of T. Uses eggshells reflection system to
        /// find the correct converter, and uses it to convert the string to T. Returns false
        /// if it failed to convert.
        /// </summary>
        public static bool TryConvert<T>(this string value, out T output)
        {
            try
            {
                output = Convert<T>(value);
                return output != null;
            }
            catch (Exception)
            {
                output = default;
                return false;
            }
        }

        /// <summary>
        /// Converts a string to an object. Based off the inputted type. Does use a little
        /// bit of reflection, which might reflect on its performance.
        /// </summary>
        public static object Convert(this string value, Type type)
        {
            if (type.IsEnum)
            {
                return Enum.Parse(type, value);
            }

            var target = typeof(IConverter<>).MakeGenericType(type);

            var converter = Library.Database.Find(target)?.Create();
            Assert.IsNull(converter, "No Valid Converters for this Type");

            return target.GetMethod("Convert")?.Invoke(converter, new object[] { value });
        }

        /// <summary>
        /// Trys to converts a string to an object. Based off the inputted type. Does use a
        /// little bit of System.Reflection, which might reflect on its performance. Returns
        /// false if it failed to convert.
        /// </summary>
        public static bool TryConvert(this string value, Type type, out object output)
        {
            try
            {
                output = Convert(value, type);
                return output != null;
            }
            catch (Exception)
            {
                output = default;
                return false;
            }
        }
    }
}
