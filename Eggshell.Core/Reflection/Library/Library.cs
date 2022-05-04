using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Eggshell.Components;
using Eggshell.Reflection;

namespace Eggshell
{
	/// <summary>
	/// <para>
	/// The Library is Eggshells string based Reflection System.
	/// </para>
	/// <para>
	/// Libraries are used for storing meta data on a type, this includes
	/// Title, Name, Group, Icons, etc. You can also add your own data
	/// using components.
	/// We can do a lotta cool and performant shit because of this.
	/// </para>
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

		internal Library( [NotNull] Type type, string name = null )
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

			// Components can mutate Group and Title, if they haven't give it one
			Group = Group.IsEmpty( type.Namespace.ToTitleCase() );
			Title = Title.IsEmpty( type.Name.ToTitleCase() );

			Properties = new( this );
			Functions = new( this );

			foreach ( var info in Info.GetMembers( BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic ) )
			{
				switch ( info )
				{
					case PropertyInfo prop :
						CacheProperty( prop );
						break;
					case MethodInfo method :
						CacheFunction( method );
						break;
				}
			}
		}

		private void CacheProperty( PropertyInfo info )
		{
			// If this property came from a class outside the scope of ILibrary
			// Ignore it. We don't care about it. 
			if ( !IsValid( info.DeclaringType ) || info.HasAttribute<SkipAttribute>( true ) || info.GetIndexParameters().Length > 0 )
			{
				return;
			}

			var reg = Members<Property, PropertyInfo>.Registry;

			if ( reg.ContainsKey( info.DeclaringType! ) )
			{
				var potential = reg[info.DeclaringType].FirstOrDefault( e => e.Name == info.Name );

				if ( potential != null )
				{
					Properties.Add( potential );
					return;
				}
			}

			if ( info.IsDefined( typeof( PropertyAttribute ) ) )
			{
				var isStatic = info.GetMethod?.IsStatic ?? info.SetMethod.IsStatic;
				(isStatic ? Global : this).Properties.Add( info.GetCustomAttribute<PropertyAttribute>().CreateRecord( info ) );

				return;
			}

			Properties.Add( new( info, info.Name, default ) );
		}

		private void CacheFunction( MethodInfo info )
		{
			if ( !info.IsDefined( typeof( FunctionAttribute ) ) || info.HasAttribute<SkipAttribute>( true ) )
			{
				return;
			}

			var reg = Members<Function, MethodInfo>.Registry;

			if ( reg.ContainsKey( info.DeclaringType! ) )
			{
				var potential = reg[info.DeclaringType].FirstOrDefault( e => e.Name == info.Name );

				if ( potential != null )
				{
					Functions.Add( potential );
					return;
				}
			}

			var attribute = info.GetCustomAttribute<FunctionAttribute>();
			(info.IsStatic ? Global : this).Functions.Add( attribute.CreateRecord( info ) );
		}

		//
		// Meta
		//

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

		//
		// Properties
		// 

		/// <summary>
		/// All the properties on this library. These are the members
		/// that have the Property attribute on them. They are used for
		/// serialization.
		/// </summary>
		public Members<Property, PropertyInfo> Properties { get; private set; }

		/// <summary>
		/// All the functions on this library. These are the members
		/// that have the Function attribute on them. 
		/// </summary>
		public Members<Function, MethodInfo> Functions { get; private set; }


	}
}
