﻿using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Eggshell.Generator
{
	public abstract class Member<T> : Member where T : class, ISymbol
	{
		protected new T Symbol => base.Symbol as T;
		protected Member( ISymbol symbol ) : base( symbol ) { }
	}

	/// <summary>
	/// Creates a member from a Symbol, that is used in Eggshells
	/// compile time reflection system.
	/// </summary>
	public abstract class Member : IMeta
	{
		protected ISymbol Symbol { get; }

		public Member( ISymbol symbol )
		{
			Symbol = symbol;

			Name = OnName( symbol );
			Title = OnTitle( symbol );
			Group = OnGroup( symbol );

			Help = Factory.Documentation( symbol );
		}

		protected virtual string OnName( ISymbol symbol )
		{
			return symbol.Name.ToProgrammerCase();
		}

		protected virtual string OnTitle( ISymbol symbol )
		{
			var attribute = symbol.GetAttributes().FirstOrDefault( e => e.AttributeClass!.Name.StartsWith( "Title" ) );

			if ( attribute is { ConstructorArguments.Length: > 0 } )
				return (string)attribute.ConstructorArguments[0].Value;

			return CultureInfo.CurrentCulture.TextInfo.ToTitleCase( string.Concat( symbol.Name.Select( x => char.IsUpper( x ) ? " " + x : x.ToString() ) ).TrimStart( ' ' ) );
		}

		protected virtual string OnGroup( ISymbol symbol )
		{
			var group = (string)symbol.GetAttributes().FirstOrDefault( e => e.AttributeClass!.Name.StartsWith( "Group" ) )?.ConstructorArguments[0].Value;
			group ??= symbol.ContainingType.Name;
			return group;
		}

		public abstract string Compile( out string className );

		// Meta Data

		public string Name { get; }
		public string Help { get; }
		public string Title { get; }
		public string Group { get; }
	}
}