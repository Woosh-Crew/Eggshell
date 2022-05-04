using System;
using Eggshell.IO;
using Eggshell.Components;
using Eggshell.Reflection;

namespace Eggshell
{
	public class KeywordAttribute : Attribute, IComponent<Function>
	{
		public string Key { get; }

		public KeywordAttribute( string key )
		{
			Key = key;
		}

		public void OnAttached( Function item )
		{
			if ( item.Info.ReturnType != typeof( string ) )
			{
				Terminal.Log.Error( $"Keyword [{Key}] on Function {item.Name} doesn't have a string return type!" );
				return;
			}

			if ( item.Info.GetParameters().Length == 0 )
			{
				Pathing.Add( Key, _ => item.Invoke<string>( null ) );
				return;
			}

			Pathing.Add( Key, args => item.Invoke<string>( null, args ) );
		}
	}
}
