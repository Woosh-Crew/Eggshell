namespace Eggshell.Resources
{
	public class Meta : IComponent<IAsset>
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string Author { get; set; }

		public void OnAttached( IAsset item ) { }
	}
}
