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
		public static void Run( string name )
		{
			Library.Database[name]?.Create<Operator>().Execute();
		}

		public static void Run<T>( string name, T callback ) where T : Delegate
		{
			Library.Database[name]?.Create<Operator<T>>().Execute( callback );
		}
		
		public Library ClassInfo => GetType();

		// Operator

		public virtual bool Valid() { return true; }

		public void Execute()
		{
			OnExecute();
		}

		protected abstract void OnExecute();
	}
}
