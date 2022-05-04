namespace Eggshell
{
	[Group( "Files" )]
	public interface IDeserializer<out T> : ILibrary
	{
		T Deserialize( byte[] item );
	}
}
