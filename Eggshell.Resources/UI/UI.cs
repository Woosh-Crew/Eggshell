using System.IO;

namespace Eggshell.Resources.UI
{
	public class Template
	{
		public Template()
		{
			Assets.Load<UI>( "template_path" );
		}
	}

	public class IElement
	{
		public string Name { get; }

		public IElement Parent { get; }
		public IElement[] Children { get; }
	}

	public class Document
	{
		public IElement Root { get; set; }

		public T Query<T>( string id ) where T : IElement
		{
			return null;
		}
	}

	[Library( "resource.ui" ), Group( "UI" )]
	public class UI : IAsset
	{
		public Library ClassInfo { get; }
		public Resource Resource { get; set; }

		public UI()
		{
			ClassInfo = Library.Register( this );
		}

		public Format Bundle { get; private set; }
		public Document Document { get; private set; }

		bool IAsset.Setup( string extension )
		{
			return true;
		}

		public virtual void Load( Stream stream )
		{
			Bundle.Load( stream );
			Document = Bundle.Build();
		}

		public virtual void Unload()
		{
			Bundle.Unload();
		}

		public IAsset Clone()
		{
			return this;
		}

		[Library( "ui.file" ), Group( "UI" )]
		public abstract class Format : ILibrary
		{
			public Library ClassInfo { get; }

			protected Format()
			{
				ClassInfo = Library.Register( this );
			}

			public abstract void Load( Stream stream );
			public abstract Document Build();
			public abstract void Unload();
		}
	}
}
