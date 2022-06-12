﻿using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Eggshell.Generator
{
    /// <summary>
    /// Creates a property from a Symbol, that is used in Eggshells
    /// compile time reflection system.
    /// </summary>
    public sealed class Property : Member<IPropertySymbol>
    {
        public Property(ISymbol symbol) : base(symbol)
        {
            Owner = Factory.OnType(symbol.ContainingType, symbol.ContainingType.IsGenericType);

            if (symbol.ContainingType.IsGenericType)
            {
                var type = symbol.ContainingType;

                // Build Generic
                var builder = new StringBuilder("<");

                for (var i = 0; i < type.TypeArguments.Length; i++)
                {
                    var para = type.TypeArguments[i];
                    builder.Append(i == 0 ? $"{para}" : $",{para}");
                }

                Owner += builder.Append('>').ToString();
            }
        }

        public override string Compile(out string className)
        {
            className = $"{Factory.OnType(Symbol.ContainingType, true)}.{Symbol.Name}".Replace('.', '_');
            var generic = Factory.OnType(Symbol.Type);

            return $@"
[CompilerGenerated]
private class {className} : Property<{generic}>
{{
	public {className}() : base( ""{Name}"", ""{Symbol.Name}"" )
	{{
		Title = ""{Title}"";
		Group = ""{Group}"";
		Help = @""{Help}"";
		IsStatic = {(Symbol.IsStatic ? "true" : "false")};
		Type = typeof( {Factory.OnType(Symbol.Type)} );
	}}

	protected override object Get( object from )
	{{
		{OnGetter()}
	}}

	public override bool IsReadable => {(Symbol.GetMethod != null ? "true" : "false")};
	public override bool IsAssignable => {(Symbol.SetMethod != null ? "true" : "false")};

	protected override void Set( object target, {generic} value )
	{{
		{OnSetter()}
	}}
}}
";
        }

        protected override string OnName(ISymbol symbol)
        {
            var attribute = symbol.GetAttributes().FirstOrDefault(e => e.AttributeClass!.Name.StartsWith("Property"));

            if (attribute is { ConstructorArguments.Length: > 0 })
                return (string)attribute.ConstructorArguments[0].Value;

            return base.OnName(symbol);
        }

        // Property

        public string Owner { get; }

        private string OnGetter()
        {
            if (Symbol.GetMethod == null || Symbol.GetMethod.DeclaredAccessibility == Accessibility.Private)
                return "return default;";

            return $@"return {(Symbol.IsStatic ? $"{Owner}.{Symbol.Name}" : $"(({Owner})from).{Symbol.Name}")};";
        }

        private string OnSetter()
        {
            if (Symbol.SetMethod == null || Symbol.SetMethod.DeclaredAccessibility is Accessibility.Private or Accessibility.Protected)
                return $@"Terminal.Log.Warning( ""Can't set {Name}, {(Symbol.SetMethod == null ? "property doesnt have setter" : "property is private")}"" );";

            var type = Factory.OnType(Symbol.Type);
            return $@"{(Symbol.IsStatic ? $"{Owner}.{Symbol.Name} = value" : $"(({Owner})target).{Symbol.Name} = value")};";
        }

        // Static

        public static bool IsValid(ISymbol symbol, ISymbol typeSymbol)
        {
            return symbol.Kind == SymbolKind.Property
                   && !symbol.Name.StartsWith("this")
                   && !symbol.GetAttributes().Any(e => e.AttributeClass.Name.StartsWith("Skip"))
                   && !symbol.IsOverride
                   && symbol.ContainingType.Equals(typeSymbol, SymbolEqualityComparer.Default)
                   && (symbol.DeclaredAccessibility == Accessibility.Public || symbol.GetAttributes().Any(attribute =>
                   {
                       var name = attribute.AttributeClass!.Name;
                       return name.StartsWith("Property");
                   }));
        }
    }
}
