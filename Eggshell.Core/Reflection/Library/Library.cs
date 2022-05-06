using System;
using Eggshell.Reflection;

namespace Eggshell
{
	/// <summary>
	/// Libraries are used for storing meta data on a type, this includes
	/// Title, Name, Group, Icons, etc. You can also add your own data
	/// using components. We can do a lotta cool and performant
	/// shit because of this.
	/// </summary>
	[Serializable, Group( "Reflection" )]
	public sealed partial class Library : ILibrary, IMeta
	{
		/// <summary>
		/// The type this library was generated for, caching
		/// its meta data in the constructor. 
		/// </summary>
		public Type Info { get; }

		/// <summary>
		/// Library's ILibrary implementation for Library. as ironic
		/// as that sounds. Its used for getting meta about Library
		/// </summary>
		public Library ClassInfo => typeof( Library );

		/// <summary>
		/// Components are added meta data onto that library, this can
		/// include icons, company, stylesheet, etc. They allow us
		/// to do some really crazy cool shit
		/// </summary>
		public Components<Library> Components { get; }

		public Library( Type type, string name )
		{
			Assert.IsNull( type );

			Info = type;
			Name = name;
			Id = Name.Hash();

			Components = new( this );
		}

		/// <summary>
		/// The programmer friendly name of this type, that is used
		/// to generate a deterministic hash for the ID.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The deterministic id created from <see cref="Library.Name"/>,
		/// which is used in a Binary Tree / Sorted List to get the library
		/// by name, and also is used for type checking for compiled data.
		/// </summary>
		public int Id { get; }

		/// <summary>
		/// The nice looking name for this library. Used in UI as well as
		/// response from the user if they don't like looking at weird names
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// The group that this library belongs to. This is used for getting
		/// all libraries, as well as other functionality depending on the class
		/// that is inherited from a specific type (like asset)
		/// </summary>
		public string Group { get; set; }

		/// <summary>
		/// The help message that tells users about this library. This is usually
		/// generated from the XML documentation above a type. Make sure your classes
		/// are documented so they are readable by both programmers and people who use
		/// your application!
		/// </summary>
		public string Help { get; set; }

		/// <summary>
		/// Prevents this Library from being constructed through the
		/// library's creator. this is false on abstract classes. Or can be
		/// disabled by you the programmer.
		/// </summary>
		public bool Spawnable { get; set; }
	}
}
