using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Eggshell.Generator
{
	public class Factory
	{
		public static string Documentation( ISymbol symbol )
		{
			var syntax = symbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax().GetLeadingTrivia().Where( e => e.IsKind( SyntaxKind.SingleLineCommentTrivia ) ).Select( e => e.ToFullString() ).ToArray();

			if ( syntax == null || syntax.Length == 0 || !syntax.Any( e => e.Contains( "<summary>" ) ) )
				return "n/a";

			// This is so fucking aids... dont change it
			return string.Join( "", syntax ).Replace( "<summary>", "" ).Replace( "</summary>", "" ).Replace( "///", "" ).Replace( "//", "" ).Replace( "\"", "\"\"" ).Trim().Replace( "\n", " " );
		}

		public static string OnType( ITypeSymbol inputType, bool ignoreGeneric = false, bool fillGenerics = true )
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

			if ( inputType is not INamedTypeSymbol { IsGenericType: true } nType || ignoreGeneric )
				return typeName.Append( inputType.Name ).ToString();

			if ( !fillGenerics )
			{
				return $"{typeName.Append( inputType.Name )}<>";
			}

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
