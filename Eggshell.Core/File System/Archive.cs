using Eggshell.IO;
using Eggshell.Reflection;

namespace Eggshell
{
    /// <summary>
    /// An archive is a statically typed file. We use this in conjunction
    /// with the library system to get types based on their extension.
    /// </summary>
    [Binding(Type = typeof(Library))]
    public partial class Archive
    {
        /// <summary>
        /// What file should we load by default if loading one failed.
        /// </summary>
        [property: Override("public string Fallback { get; set; }")]
        public Pathing Fallback { get; set; }

        /// <summary>
        /// The default extension for this archive.
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// The serialization type for this archive.
        /// </summary>
        public Serialization Serialization { get; set; }
    }
}
