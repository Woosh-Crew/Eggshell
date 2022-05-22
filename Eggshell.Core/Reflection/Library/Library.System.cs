using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Eggshell
{
	public partial class Library
	{
		/// <summary>
		/// Database for Library Records. Allows the access of all records.
		/// Use extension methods to add functionality to database access.
		/// </summary>
		public static Libraries Database { get; private set; }

		private static Dictionary<Type, ILibrary> Singletons { get; } = new();

		static Library()
		{
			Database = new();

			var stopwatch = Stopwatch.StartNew();

			Database.Add( AppDomain.CurrentDomain );

			stopwatch.Stop();
			Terminal.Log.Info( $"Library Ready | {stopwatch.Elapsed.TotalMilliseconds}ms" );
		}

		internal static bool IsValid( Type type )
		{
			return type.HasInterface<ILibrary>() || type.IsDefined( typeof( LibraryAttribute ), true );
		}

		/// <summary>
		/// Registers the target object with the Library.
		/// Which allows it to receive instance callbacks and
		/// returns its library instance.
		/// </summary>
		public static Library Register( ILibrary value )
		{
			Library lib = value.GetType();
			Assert.IsNull( lib );

			if ( IsSingleton( lib ) )
			{
				if ( Singletons.ContainsKey( lib.Info ) )
				{
					Terminal.Log.Error( $"You're trying to register another Singleton [{lib.Name}]" );
					return null;
				}

				Singletons.Add( lib.Info, value );
			}

			lib.OnRegister( value );
			return lib;
		}

		/// <summary>
		/// Cleans up ILibrary object, removes it from instance
		/// callback database so the garbage collector picks it up.
		/// </summary>
		public static void Unregister( ILibrary value )
		{
			// Check if Library is Singleton
			if ( IsSingleton( value.ClassInfo ) )
			{
				Singletons.Remove( value.GetType() );
			}

			value.ClassInfo.OnUnregister( value );
		}

		/// <summary>
		/// Checks if the lib has a singleton component,
		/// and an instance of it somewhere.
		/// </summary>
		public static bool IsSingleton( Library lib )
		{
			return lib.Components.Has<SingletonAttribute>() && !lib.Info.HasInterface( typeof( IComponent ) );
		}

		/// <summary> <inheritdoc cref="Create"/> and returns T </summary>
		public static T Create<T>( Library lib = null )
		{
			lib ??= typeof( T );
			return (T)lib.Create();
		}

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
