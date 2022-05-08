using System;
using System.Reflection;

namespace Eggshell.Reflection
{
	/// <summary>
	/// Properties are used in libraries for defining variables
	/// that can be inspected and changed.
	/// </summary>
	[Serializable, Group( "Reflection" )]
	public class Function : ILibrary, IMember<MethodInfo>
	{
		/// <summary> 
		/// The MethodInfo that this property was generated for. 
		/// caching its meta data in the constructor.
		/// </summary>
		public MethodInfo Info { get; }

		/// <summary>
		/// Where did this function come from? (This will automatically
		/// pull function from this Library) - (Data is filled
		/// out by source generators)
		/// </summary>
		public Library Parent { get; set; }

		/// <summary>
		/// Function's ILibrary implementation for Library. as ironic
		/// as that sounds. Its used for getting meta about the Property
		/// </summary>
		public Library ClassInfo => typeof( Property );

		/// <summary>
		/// It isn't recommended that you create a function manually, as
		/// this is usually done through source generators.
		/// </summary>
		public Function( string name, MethodInfo info )
		{
			Assert.IsNull( info );

			Info = info;

			Name = name;
			Id = Name.Hash();
		}

		/// <summary>
		/// The programmer friendly name of this function, that is used
		/// to generate a deterministic hash for the ID.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The deterministic id created from <see cref="Property.Name"/>,
		/// which is used in a Binary Tree / Sorted List to get functions
		/// by name.
		/// </summary>
		public int Id { get; }

		/// <summary>
		/// The nice looking name for this function. Used in UI as well as
		/// response from the user because they don't like looking at weird names
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// The help message that tells users about this function. This is usually
		/// generated from the XML documentation above a function.
		/// </summary>
		public string Help { get; set; }

		/// <summary>
		/// The group that this function belongs to. This is used for getting
		/// all functions by group, as well as other functionality depending
		/// on the class that this function is from.
		/// </summary>
		public string Group { get; set; }

		/// <summary>
		/// Does this function require an instance accessor to be invoked?
		/// </summary>
		public bool IsStatic => Info.IsStatic;
	}
}
