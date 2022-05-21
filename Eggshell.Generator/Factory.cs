using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Eggshell.Generator
{
	public class Factory
	{
		public static string OnType( ITypeSymbol inputType )
		{
			// Build Array
			if ( inputType is IArrayTypeSymbol arrayTypeSymbol )
			{
				return OnType( arrayTypeSymbol.ElementType ) + "[]";
			}

			var typeName = new StringBuilder( inputType.ContainingNamespace != null ? $"{inputType.ContainingNamespace}." : string.Empty );

			// This should be inverted
			var currentType = inputType.ContainingType;
			if ( currentType != null )
			{
				var symbols = new List<INamedTypeSymbol>();

				while ( currentType != null )
				{
					symbols.Add( currentType );
					currentType = currentType.ContainingType;
				}

				symbols.Reverse();

				foreach ( var type in symbols )
				{
					typeName.Append( $"{type.Name}." );
				}

				symbols.Clear();
			}

			if ( inputType is not INamedTypeSymbol { IsGenericType: true } nType )
				return typeName.Append( inputType.Name ).ToString();

			// Build Generic
			var builder = new StringBuilder( "<" );

			for ( var i = 0; i < nType.TypeArguments.Length; i++ )
			{
				var fullName = $"{(nType.TypeArguments[i].ContainingNamespace != null ? $"{nType.TypeArguments[i].ContainingNamespace}." : string.Empty)}{nType.TypeArguments[i].Name}";
				builder.Append( i == 0 ? $"{fullName}" : $",{fullName}" );
			}

			return $"{typeName.Append( inputType.Name )}{builder.Append( '>' )}";
		}
	}
}
