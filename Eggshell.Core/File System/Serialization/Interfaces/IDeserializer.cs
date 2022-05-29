namespace Eggshell
{
	[Group( "Files" )]
	public interface IDeserializer<out T> : IObject
	{
		T Deserialize( byte[] item );
	}
}
