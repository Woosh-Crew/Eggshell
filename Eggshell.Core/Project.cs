using System;
using System.Reflection;

namespace Eggshell
{
	/// <summary>
	/// Entry point to your Eggs, You'd want to
	/// inherit from this so Eggshell automatically
	/// creates your applications object. Make sure to
	/// call Crack in your programs entry point.
	/// </summary>
	public abstract class Project : Module
	{
		/// <summary>
		/// Initializes all Modules, created by
		/// source generators, through reflection
		/// </summary>
		protected static void Crack()
		{
			// Cache Modules
			foreach ( var assembly in AppDomain.CurrentDomain.GetAssemblies() )
			{
				assembly.GetType( "Eggshell.Generated.Modules" )?.GetMethod( "Cache", BindingFlags.Static | BindingFlags.NonPublic )?.Invoke( null, null );
			}
		}
	}
}
