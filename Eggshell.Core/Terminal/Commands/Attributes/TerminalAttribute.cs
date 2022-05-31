using System;
using System.Linq;
using Eggshell.Debugging;
using Eggshell.Reflection;

namespace Eggshell
{
	/// <summary>
	/// A Terminal is either a Var or Function that can be invoked through Eggshells
	/// debugging library. This allows us to easily change the values or invoke functions
	/// inside libraries, instanced or not.
	/// </summary>
	[AttributeUsage( AttributeTargets.Property | AttributeTargets.Method, Inherited = false )]
	public sealed class TerminalAttribute : Attribute
	{
		/* 
		public void OnAttached( Function item )
		{
			if ( !item.Info.IsStatic )
			{
				Terminal.Log.Error( $"Function \"{item.Name}\" Must be Static!" );
				return;
			}

			var command = new Command()
			{
				Member = item,
				Info = item.Info,
				Parameters = item.Info.GetParameters().Select( e => e.ParameterType ).ToArray()
			}.WithAction(
				( e ) => item.Info?.Invoke( null, e )
			);

			Terminal.Command.Push( command );
		}

		public void OnAttached( Property item )
		{
			if ( !item.IsStatic )
			{
				Terminal.Log.Error( $"Property \"{item.Name}\" Must be Static!" );
				return;
			}

			var command = new Command()
			{
				Member = item,
				Info = item.Info,
				Parameters = new[] { item.Info.PropertyType }
			}.WithAction(
				( parameters ) =>
				{
					if ( parameters is { Length: > 0 } )
					{
						var value = parameters[0];
						item[null] = value;

						Terminal.Log.Info( $"{item.Name} now equals {value}" );
					}
					else
					{
						Terminal.Log.Info( $"{item.Name} = {item[null]}" );
					}
				}
			);

			Terminal.Command.Push( command );
		}
		*/
	}
}
