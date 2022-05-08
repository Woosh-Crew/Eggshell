namespace Eggshell.Tests
{
	public class Console : Application
	{
		public static void Main( string[] args )
		{
			Crack();

			foreach ( var library in Library.Database )
			{
				Terminal.Log.Info( $"{library.Title} - {library.Help}" );

				foreach ( var property in library.Properties )
				{
					Terminal.Log.Info( $"{property.Name} - {property.Help} - from: {property.Parent.Name}" );
				}

				Terminal.Log.Info( "---------------------------" );
			}
		}
	}
}
