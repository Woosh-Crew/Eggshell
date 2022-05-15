using System;

namespace Eggshell.Reflection
{
	public class DynamicProperty : Property
	{
		public DynamicProperty( string name ) : base( name, null ) { }

		public Func<object, object> Getter { get; set; }
		public Action<object, object> Setter { get; set; }

		protected override object Get( object from )
		{
			return Getter?.Invoke( from );
		}

		protected override void Set( object value, object target )
		{
			Setter.Invoke( value, target );
		}
	}
}
