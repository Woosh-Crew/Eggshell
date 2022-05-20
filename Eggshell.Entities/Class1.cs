namespace Eggshell.Entities
{
	public class Entity : ILibrary
	{
		public Library ClassInfo { get; }

		public Entity()
		{
			ClassInfo = Library.Register( this );
		}
	}
}
