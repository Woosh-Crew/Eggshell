namespace Eggshell
{
	[Group( "Files" )]
	public interface ISerializer<in T> : IObject
	{
		byte[] Serialize( T item );
		byte[] Serialize( T[] item );
	}
}
