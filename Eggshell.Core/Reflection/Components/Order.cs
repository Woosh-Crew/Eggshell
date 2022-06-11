using Eggshell.Reflection;

namespace Eggshell
{
    /// <summary>
    /// Allows a member to have injected components into it. Basically
    /// builds the source tree to allow custom logic to be injected
    /// </summary>
    [Binding(Type = typeof(Library))]
    public partial class Order
    {
        /// <summary>
        /// The value that was created by the attribute that signifies
        /// what order this class or what ever should be in.
        /// </summary>
        [Skip] public int Value { get; }

        public Order(int order)
        {
            Value = order;
        }
    }
}
