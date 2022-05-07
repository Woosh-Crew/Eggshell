using System;
using System.Reflection;

namespace Eggshell.Reflection
{
	/// <summary>
	/// Properties are used in libraries for defining variables
	/// that can be inspected and changed.
	/// </summary>
	[Serializable, Group( "Reflection" )]
	public class Property : ILibrary, IMember<PropertyInfo>
	{
		/// <summary> 
		/// The PropertyInfo that this property was generated for. 
		/// caching its meta data in the constructor.
		/// </summary>
		public PropertyInfo Info { get; }

		/// <summary>
		/// Where did this property come from? (This will automatically
		/// pull properties and methods from this Library) - (Data is filled
		/// out by source generators)
		/// </summary>
		public Library Parent { get; set; }

		/// <summary>
		/// Property's ILibrary implementation for Library. as ironic
		/// as that sounds. Its used for getting meta about the Property
		/// </summary>
		public Library ClassInfo => typeof( Property );

		/// <summary>
		/// It isn't recommended that you create a property manually, as
		/// this is usually done through source generators.
		/// </summary>
		public Property( string name, PropertyInfo info, Library parent = null )
		{
			Assert.IsNull( info );

			Info = info;

			Name = name;
			Id = Name.Hash();

			Parent = parent;
		}

		/// <summary>
		/// Tells the target instance to change the properties value
		/// by the input of "from".
		/// </summary>
		/// <param name="from"></param>
		public object this[ object from ]
		{
			get => Info.GetMethod == null ? default : Info.GetValue( from );
			set => Info.SetValue( from, value );
		}

		/// <summary>
		/// The programmer friendly name of this property, that is used
		/// to generate a deterministic hash for the ID.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The deterministic id created from <see cref="Property.Name"/>,
		/// which is used in a Binary Tree / Sorted List to get properties
		/// by name.
		/// </summary>
		public int Id { get; }

		/// <summary>
		/// The nice looking name for this Property. Used in UI as well as
		/// response from the user because they don't like looking at weird names
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// The help message that tells users about this property. This is usually
		/// generated from the XML documentation above a property.
		/// </summary>
		public string Help { get; set; }

		/// <summary>
		/// The group that this property belongs to. This is used for getting
		/// all property by group, as well as other functionality depending
		/// on the class that this property is from.
		/// </summary>
		public string Group { get; set; }

		/// <summary>
		/// Does this property require an instance accessor to be changed?
		/// Used in the value setter.
		/// </summary>
		public bool IsStatic => Info.GetMethod?.IsStatic ?? Info.SetMethod!.IsStatic;
	}
}
