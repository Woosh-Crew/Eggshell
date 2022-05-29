namespace Eggshell.Converters
{
	[Group( "Converters" )]
	public interface IConverter<out T> : IObject
	{
		T Convert( string value );
	}
}
