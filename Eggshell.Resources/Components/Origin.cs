namespace Eggshell.Resources
{
	public class Origin : IComponent<IAsset>
	{
		public string Name { get; set; }

		public void OnAttached( IAsset item ) { }
	}
}
