namespace Eggshell.Converters
{
	[Group( "Converters" )]
	public interface IConverter<out T> : ILibrary
	{
		T Convert( string value );
	}
}
