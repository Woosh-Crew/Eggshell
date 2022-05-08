using System;
using System.Collections.Generic;
using System.Reflection;
using Eggshell.Reflection;

namespace Eggshell
{
	/// <summary>
	/// Libraries are used for storing meta data on a type, this includes
	/// Title, Name, Group, Icons, etc. You can also add your own data
	/// using components. We can do a lotta cool and performant
	/// shit because of this. Such as easily doing C# / C++ calls.
	/// </summary>
	[Serializable, Group( "Reflection" )]
	public partial class Library : ILibrary, IMeta
	{
		/// <summary>
		/// The type this library was generated for, caching
		/// its meta data in the constructor. 
		/// </summary>
		public Type Info { get; }

		/// <summary>
		/// What is this library inherited from. (This will automatically
		/// pull properties and methods from this Library) - (Data is filled
		/// out by source generators)
		/// </summary>
		public Library Parent { get; }

		/// <summary>
		/// What is using this Library for inheritance. (Data is filled
		/// out by source generators)
		/// </summary>
		public List<Library> Children { get; } = new();

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

		/// <summary>
		/// Properties are variables that the library owns. This is
		/// usually mutable value types if the property has a setter
		/// </summary>
		public Members<Property> Properties { get; }

		/// <summary>
		/// Functions are tasks that the library does broken up into
		/// groups so its easier to digest how a program works.
		/// </summary>
		public Members<Function> Functions { get; }

		/// <summary>
		/// It isn't recommended that you create the library manually, as
		/// this is usually done through source generators.
		/// </summary>
		public Library( string name, Type type, Library parent = null )
		{
			Assert.IsNull( type );

			Info = type;

			Properties = new( this );
			Functions = new( this );

			Name = name;
			Id = Name.Hash();

			Parent = parent;

			if ( Parent != null )
			{
				Parent.Children.Add( this );

				// Inherit Members
				Properties.Add( Parent.Properties );
				Functions.Add( Parent.Functions );
			}

			Components = new( this );
		}

		/// <summary>
		/// Creates an ILibrary, this just does some sanity checking before
		/// calling the internal Construct() method, which can be overridden
		/// or uses a constructor attribute.
		/// </summary>
		public ILibrary Create()
		{
			if ( Spawnable )
			{
				return IsSingleton( this ) && Singletons.ContainsKey( Info ) ? Singletons[Info] : Construct();
			}

			Terminal.Log.Error( $"{Name} is not spawnable. Set Spawnable to true in classes meta." );
			return null;
		}

		/// <summary>
		/// Constructs this ILibrary, can be overriden to provide custom logic.
		/// such as using a custom constructor, or setting off events when
		/// this library has been constructed. Use wisely!
		/// </summary>
		protected virtual ILibrary Construct()
		{
			// Check if we have a custom Constructor
			if ( Components.TryGet<ConstructorAttribute>( out var constructor ) )
			{
				return (ILibrary)constructor.Invoke();
			}

			if ( !Info.IsAbstract )
			{
				return (ILibrary)Activator.CreateInstance( Info );
			}

			Terminal.Log.Error( $"Can't construct {Name}, is abstract and doesn't have constructor predefined." );
			return null;
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
