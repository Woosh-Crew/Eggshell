using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
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
		public Library ClassInfo => typeof( Library );

		/// <summary>
		/// Components are added meta data onto that library, this can
		/// include icons, company, stylesheet, etc. They allow us
		/// to do some really crazy cool shit
		/// </summary>
		public Components<Library> Components { get; }

		public Library( [NotNull] Type type, string name = null )
		{
			Assert.IsNull( type );

			Info = type;
			Name = name.IsEmpty( type.Name.ToProgrammerCase( type.Namespace ) );
			Id = Name.Hash();

			Components = new( this );

			// This is really expensive (6ms)...
			// Get Components attached to type
			var attributes = Info.GetCustomAttributes();
			foreach ( var item in attributes )
			{
				if ( item is IComponent<Library> library )
				{
					Components.Add( library );
				}
			}

			Group = Group.IsEmpty( type.Namespace.ToTitleCase() );
			Title = Title.IsEmpty( type.Name.ToTitleCase() );
		}

		public Type Info { get; }
		public string Name { get; }
		public int Id { get; }
		public string Title { get; set; }
		public string Group { get; set; }
		public string Help { get; set; }

		public bool Spawnable { get; set; } = true;

		public static implicit operator Library( string value )
		{
			return Database[value];
		}

		public static implicit operator Library( Type value )
		{
			return Database[value];
		}

		public static implicit operator Library( int hash )
		{
			return Database[hash];
		}
	}
}
