using System.Collections;
using System.Collections.Generic;

namespace Eggshell
{
	/// <summary>
	/// This class is a helper class for throwing exceptions
	/// when an operation is invalid.
	/// </summary>
	public static class Assert
	{
		public static void IsEmpty( ICollection collection, string message = "Collection was Empty!" )
		{
			if ( collection.Count == 0 )
			{
				Fail( message );
			}
		}

		public static void IsNull<T>( T item, string message = "Item was NULL!" )
		{
			if ( item == null )
			{
				Fail( message );
			}
		}

		public static void IsNotNull<T>( T item, string message = "Item was not NULL!" )
		{
			if ( item != null )
			{
				Fail( message );
			}
		}

		public static void Missing<T>( IList<T> collection, T item, string message = "Database doesn't contain item!" )
		{
			if ( !collection.Contains( item ) )
			{
				Fail( message );
			}
		}

		public static void Contains<T>( IList<T> collection, T item, string message = "Database already contains item!" )
		{
			if ( collection.Contains( item ) )
			{
				Fail( message );
			}
		}

		// Bool

		public static void IsEqual<T>( T item, T comparison, string message = null )
		{
			if ( item.Equals( comparison ) )
			{
				Fail( message );
			}
		}

		public static void IsTrue( bool item, string message = "Bool was True!" )
		{
			if ( item )
			{
				Fail( message );
			}
		}

		public static void IsFalse( bool item, string message = "Bool was False!" )
		{
			if ( item == false )
			{
				Fail( message );
			}
		}

		// Utility

		private static void Fail( string message )
		{
			throw new( message );
		}
	}
}
