using Eggshell.Reflection;

namespace Eggshell.IO
{
    /// <summary>
    /// Registers a Path to be unusable in the pathing system. is used like 
    /// {Short://}.
    /// </summary>
    [Binding(Type = typeof(Library))]
    public partial class Pathable
    {
        /// <summary>
        /// The Shorthand that is used when registering the path and then 
        /// used like this "{value}://", excellent for quickly getting the path
        /// of something, without knowing where it actually is.
        /// </summary>
        [Skip] public string Short { get; }

        /// <summary>
        /// The full path that the short path maps too, you can nest short paths 
        /// in this full path. They will unravel when asking for the path.
        /// </summary>
        [Skip] public Pathing Full { get; }

        /// <summary>
        /// Sets up the component, Hand = Shorthand, Full = Absolute path that allows
        /// nested shorthand. 
        /// </summary>
        public Pathable(string hand, string full)
        {
            Short = hand;
            Full = full;
        }

        public void OnAttached()
        {
            Pathing.Add(Short, Full);
        }
    }
}
