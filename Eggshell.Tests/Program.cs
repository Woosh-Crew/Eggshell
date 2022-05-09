using Eggshell.Resources;

namespace Eggshell.Tests
{
	public class Console : Project
	{
		public static void Main( string[] args )
		{
			Assets.Find( 0 );
			Crack();

			foreach ( var library in Library.Database )
			{
				Terminal.Log.Info( library.Name );
				
				foreach ( var property in library.Properties )
				{
					Terminal.Log.Info( $"-- {property.Name}" );
				}
			}
		}
	}
}
