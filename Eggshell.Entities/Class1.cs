namespace Eggshell.Entities
{
	public class Entity : IObject
	{
		public Library ClassInfo { get; }

		public Entity()
		{
			ClassInfo = Library.Register( this );
		}
	}
}
