namespace Eggshell
{
    /// <summary>
    /// An Object having this interface allows it to be possessed by
    /// a client. Allowing a client to have authority over it.
    /// </summary>
    public interface IPossessable
    {
        /// <summary>
        /// A callback for when a client takes authority over the
        /// object.
        /// </summary>
        void OnPossess();
        
        /// <summary>
        /// A callback for when a client doesnt have authority over the
        /// object any more.
        /// </summary>
        void OnUnpossess();
    }
}
