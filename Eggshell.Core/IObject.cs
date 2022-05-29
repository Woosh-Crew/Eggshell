namespace Eggshell
{
	/// <summary>
	/// Stores class information, used for networking, meta data, etc.
	/// Which can be accessed in the Library.Database
	/// </summary>
	public interface IObject
	{
		/// <summary>
		/// ClassInfo holds this classes meta data, that is stored in the Library Cache.
		/// Library stores all Meta Data relating to this current class,
		/// such as Functions, Properties, Groups, Components, etc.
		/// </summary>
		Library ClassInfo { get; }

		/// <summary>
		/// Deletes this object from the library registry. Override this to call
		/// additional logic when deleting an IObject object.
		/// </summary>
		void Delete()
		{
			Library.Unregister( this );
		}
	}
}
