using Eggshell.Generated;

namespace Eggshell.Tests
{
	public class Console : App
	{
		public string Variable { get; set; }

		public static void Main( string[] args )
		{
			App.Crack();

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
