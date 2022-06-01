using System;
using Eggshell.Converters;

namespace Eggshell
{
	/// <summary>
	/// Eggshell's string to object converter.
	/// </summary>
	public static class Converter
	{
		/// <summary>
		/// Converts a string to the type of T. 
		/// </summary>
		public static T Convert<T>( this string value )
		{
			if ( typeof( T ).IsEnum )
			{
				return (T)Enum.Parse( typeof( T ), value );
			}

			var library = Library.Database.Find<IConverter<T>>();

			if ( library == null )
			{
				Terminal.Log.Error( $"No Valid converters for {typeof( T ).Name}." );
				return default;
			}

			var converter = Library.Create<IConverter<T>>( library );

			try
			{
				return converter.Convert( value );
			}
			catch ( Exception e )
			{
				Terminal.Log.Exception( e );
				return default;
			}
		}

		/// <summary>
		/// Converts a string to an object. Based off the
		/// inputted type. This uses reflection, and is pretty slow..
		/// Be careful where you put this method.
		/// </summary>
		public static object Convert( this string value, Type type )
		{
			// Doing explicit enum shit here, cause fuck it, this class is already painful
			if ( type.IsEnum )
			{
				return Enum.Parse( type, value );
			}

			// JAKE: This is so aids.... But can't do much about that.

			var interfaceType = typeof( IConverter<> ).MakeGenericType( type );
			var library = Library.Database.Find( interfaceType );

			Assert.IsNull( library, "No Valid Converters for this Type" );

			var converter = library.Create();
			var method = interfaceType.GetMethod( "Convert" );

			try
			{
				return method?.Invoke( converter, new object[] { value } );
			}
			catch ( Exception e )
			{
				Terminal.Log.Exception( e );
				return default;
			}
		}
	}
}
