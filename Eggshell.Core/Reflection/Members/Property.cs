using System;
using System.Reflection;

namespace Eggshell.Reflection
{
	/// <summary>
	/// An interface that supplies a generic less property
	/// for interacting with, if you don't know the type.
	/// </summary>
	public interface IProperty : IMember<PropertyInfo>
	{
		object Get( object from );
		void Set( object value, object target );
		object this[ object from ] { get; set; }
	}

	/// <summary>
	/// Properties are used in libraries for defining variables
	/// that can be inspected and changed. Every property has its
	/// own class generated for it. So it can be ultra optimised.
	/// You can also create dynamic properties that can be edited
	/// at runtime, which is incredibly useful in a handful
	/// of scenarios.
	/// </summary>
	[Serializable, Group( "Reflection" ), Skip]
	public abstract class Property<T> : IObject, IProperty
	{
		/// <summary> 
		/// The PropertyInfo that this property was generated for. 
		/// This calls GetProperty, if it hasn't already.
		/// </summary>
		public PropertyInfo Info => Origin == null ? null : _info ??= Parent.Info.GetProperty( Origin, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic );

		/// <summary>
		/// The cached property info that is generated from
		/// <see cref="Property{T}.Info"/>.
		/// </summary>
		private PropertyInfo _info;

		/// <summary>
		/// Where did this property come from? (This will automatically
		/// pull properties and methods from this Library) - (Data is filled
		/// out by source generators)
		/// </summary>
		public Library Parent { get; set; }

		/// <summary>
		/// Property's IObject implementation for Library. as ironic
		/// as that sounds. Its used for getting meta about the Property
		/// </summary>
		public Library ClassInfo => typeof( Property<T> );

		/// <summary>
		/// Components that have been attached to this property. Allows
		/// for easy storage of meta data, as well as property specific logic.
		/// </summary>
		public Components<IProperty> Components { get; }

		/// <summary>
		/// It isn't recommended that you create a property manually, as
		/// this is usually done through source generators.
		/// </summary>
		public Property( string name, string origin )
		{
			Name = name;
			Origin = origin;

			Id = Name.Hash();
		}

		/// <summary>
		/// Tells the target instance to change the properties value
		/// by the input of "from".
		/// </summary>
		public T this[ object from ]
		{
			get => Get( from );
			set => Set( from, value );
		}

		/// <summary>
		/// A helper for setting and getting static variables to a value.
		/// Behind the scenes (if not overriden) will just get the indexer
		/// by null, and return the value based off of that.
		/// </summary>
		public virtual T Value
		{
			get
			{
				if ( IsStatic )
					return this[null];

				Terminal.Log.Error( "Can't get unstatic property through value, use indexer instead." );
				return default;

			}
			set
			{
				if ( IsStatic )
				{
					this[null] = value;
					return;
				}

				Terminal.Log.Error( "Can't set unstatic property through value, use indexer instead." );
			}
		}

		protected abstract T Get( object from );
		protected abstract void Set( object from, T value );

		// IProperty

		object IProperty.this[ object from ]
		{
			get => Get( from );
			set => Set( from, (T)value );
		}

		object IProperty.Get( object from )
		{
			return Get( from );
		}

		void IProperty.Set( object value, object target )
		{
			Set( target, (T)value );
		}

		/// <summary>
		/// The name of the property member that this eggshell property
		/// was generated from. Used when getting the property itself. 
		/// </summary>
		public string Origin { get; }

		/// <summary>
		/// The programmer friendly name of this property, that is used
		/// to generate a deterministic hash for the ID.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The deterministic id created from <see cref="Property{T}.Name"/>,
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
		public virtual bool IsStatic { get; set; }

		/// <summary>
		/// The backing type that this property is using.
		/// </summary>
		public virtual Type Type { get; protected set; }

		/// <summary>
		/// A nice to have implicit operator to the target type, so if we're
		/// dealing with static properties, it's super easy to get them.
		/// </summary>
		public static implicit operator T( Property<T> property )
		{
			return property.IsStatic ? property[null] : default;
		}
	}
}
