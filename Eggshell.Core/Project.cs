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
		/// Uses Eggshells reflection system to find a
		/// bootstrap and uses that to initialize all
		/// modules created by source generators
		/// </summary>
		protected static void Crack()
		{
			Crack( Library.Create<Bootstrap>( Library.Database.Find<Bootstrap>() ) );
		}

		/// <summary>
		/// Initializes all Modules, created by
		/// source generators, through reflection
		/// </summary>
		protected static void Crack( Bootstrap bootstrap )
		{
			bootstrap.Boot();
			Terminal.Log.Info( "Eggshell Ready" );
		}
	}
}
