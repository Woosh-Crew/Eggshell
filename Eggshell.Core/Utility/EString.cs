using System;
using System.Linq;
using System.Text;
using Eggshell.IO;

namespace Eggshell
{
    public static class EString
    {
        public static int Hash(this string value)
        {
            return (int)Encoding.Unicode.GetBytes(value).Aggregate(2166136261, (current, num2) => (current ^ num2) * 16777619U);
        }

        public static bool IsEmpty(this string value)
        {
            if (value == null)
            {
                return true;
            }

            return value == string.Empty;
        }

        public static string IsEmpty(this string value, string replace)
        {
            return IsEmpty(value) ? replace : value;
        }

        public static string ReplaceAt(this string input, int index, char newChar)
        {
            Assert.IsNull(input);
            return new StringBuilder(input) { [index] = newChar }.ToString();
        }

        public static string ToTitleCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            // Change this stuff to space
            value = value.Replace("_", " ");
            value = value.Replace("-", " ");
            value = value.Replace(".", " ");

            // Set first char to upper
            value = value.ReplaceAt(0, char.ToUpper(value[0]));
            return string.Concat(value.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        }

        public static string ToProgrammerCase(this string value, string prefix = null)
        {
            var name = string.Concat(value.Select(x => char.IsUpper(x) ? "_" + x : x.ToString())).TrimStart('_');

            if (string.IsNullOrEmpty(prefix))
            {
                return name.ToLower();
            }

            name = name.Replace(' ', '_');

            prefix = prefix.Split('.')[^1] ?? "";
            return $"{prefix}.{name}".ToLower();
        }

        // Copied from this (https://stackoverflow.com/a/2132004)
        public static string[] SplitArguments(this string commandLine)
        {
            var paramChars = commandLine.ToCharArray();
            var inSingleQuote = false;
            var inDoubleQuote = false;
            for (var index = 0; index < paramChars.Length; index++)
            {
                if (paramChars[index] == '"' && !inSingleQuote)
                {
                    inDoubleQuote = !inDoubleQuote;
                    paramChars[index] = '\n';
                }

                if (paramChars[index] == '\'' && !inDoubleQuote)
                {
                    inSingleQuote = !inSingleQuote;
                    paramChars[index] = '\n';
                }

                if (!inSingleQuote && !inDoubleQuote && paramChars[index] == ' ')
                {
                    paramChars[index] = '\n';
                }
            }

            return new string(paramChars).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
