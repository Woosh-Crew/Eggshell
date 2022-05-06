using System;
using System.Reflection;

namespace Eggshell
{
	public class App : Module
	{
		/// <summary>
		/// Initializes all Eggshell Systems, from
		/// source generators using reflection
		/// </summary>
		public static void Crack()
		{
			// Cache Modules
			foreach ( var assembly in AppDomain.CurrentDomain.GetAssemblies() )
			{
				assembly.GetType( "Eggshell.Generated.Modules" )?.GetMethod( "Cache", BindingFlags.Static | BindingFlags.NonPublic )?.Invoke( null, null );
			}
		}
	}
}
