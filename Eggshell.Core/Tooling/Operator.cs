using System;

namespace Eggshell
{
	/// <summary>
	/// An operator is a operation that is performed in the editor. Operators
	/// can be searched for in the editor is well. It is also recommended you
	/// use operators for tools that do actions / operations. This operator
	/// can have a callback.
	/// </summary>
	[Library( "tools.operator_generic" )]
	public abstract class Operator<T> : Operator where T : Delegate
	{
		public void Execute( T callback )
		{
			if ( !Valid() )
			{
				Terminal.Log.Warning( $"Couldn't Execute {ClassInfo.Title}, Operator wasn't valid" );
				return;
			}

			OnExecute( callback );
		}

		protected abstract void OnExecute( T callback );
		protected sealed override void OnExecute() { Execute( null ); }
	}

	/// <summary>
	/// An operator is a operation that is performed in the editor. Operators
	/// can be searched for in the editor is well. It is also recommended you
	/// use operators for tools that do actions / operations.
	/// </summary>
	[Library( "tools.operator" )]
	public abstract class Operator : IObject
	{
		/// <summary>
		/// Runs an operator based off its Library name. Not caring about
		/// any callback that it could potentially have.
		/// </summary>
		public static void Run( string name )
		{
			Find<Operator>( name ).Execute();
		}

		/// <summary>
		/// Runs an operator based off its Library name, with a delegate
		/// generic arg, representing the callback that is invoked.
		/// </summary>
		public static void Run<T>( string name, T callback ) where T : Delegate
		{
			Find<Operator<T>>( name ).Execute( callback );
		}

		// Helpers

		/// <summary>
		/// Runs an operator based off its Library name, with a predefined
		/// action which represents the callback.
		/// </summary>
		public static void Run( string name, Action callback )
		{
			Find<Operator<Action>>( name ).Execute( callback );
		}

		/// <summary>
		/// Runs an operator based off its Library name, with a predefined
		/// action with the generic args based off the ones fed through the
		/// method, which represents the callback.
		/// </summary>
		public static void Run<T>( string name, Action<T> callback )
		{
			Find<Operator<Action<T>>>( name ).Execute( callback );
		}

		/// <summary>
		/// Runs an operator based off its Library name, with a predefined
		/// action with the generic args based off the ones fed through the
		/// method, which represents the callback.
		/// </summary>
		public static void Run<T1, T2>( string name, Action<T1, T2> callback )
		{
			Find<Operator<Action<T1, T2>>>( name ).Execute( callback );
		}

		/// <summary>
		/// Runs an operator based off its Library name, with a predefined
		/// action with the generic args based off the ones fed through the
		/// method, which represents the callback.
		/// </summary>
		public static void Run<T1, T2, T3>( string name, Action<T1, T2, T3> callback )
		{
			Find<Operator<Action<T1, T2, T3>>>( name ).Execute( callback );
		}

		/// <summary>
		/// Finds an operator based off the inputted name, creates it and
		/// returns the arg of T. So you can manually invoke it.
		/// </summary>
		public static T Find<T>( string name ) where T : Operator
		{
			return Library.Database[name]?.Create<T>();
		}

		// Operator

		public Library ClassInfo => GetType();

		public virtual bool Valid() { return true; }

		public void Execute()
		{
			if ( !Valid() )
			{
				Terminal.Log.Warning( $"Couldn't Execute {ClassInfo.Title}, Operator wasn't valid" );
				return;
			}

			OnExecute();
		}

		protected abstract void OnExecute();
	}
}
