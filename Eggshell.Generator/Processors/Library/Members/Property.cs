using System;
using System.Collections.Generic;
using System.Linq;
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

    public override void OnAttached(Library library)
    {{
        // Add Components
		{OnComponents()}
    }}

	protected override void Get( object from, out {generic} value )
	{{
		{OnGetter()}
	}}

	public override bool IsReadable => {(Symbol.GetMethod != null ? "true" : "false")};
	public override bool IsAssignable => {(Symbol.SetMethod != null ? "true" : "false")};

	protected override void Set( object target, {generic} value )
	{{
		{OnSetter()}
	}}

    // Components
	{string.Join("\n", Components)}
}}
";
        }

        public StringBuilder Components { get; } = new();
        public List<string> Bindings { get; } = new();

        private string OnComponents()
        {
            bool IsValid(INamedTypeSymbol symbol)
            {
                // Normal Symbol
                if (symbol.GetAttributes().Any(e => e.AttributeClass.Name.StartsWith("Binding")))
                {
                    return true;
                }

                // Weird shit, because roslyn is retarded

                if (symbol.Name.Contains("Attribute"))
                {
                    return false;
                }

                return Generator.Current.Compilation.GetTypeByMetadataName(Factory.OnType(symbol)).GetAttributes().Any(e => e.AttributeClass.Name.StartsWith("Binding"));
            }

            var builder = new StringBuilder();

            foreach ( var symbol in Symbol.GetAttributes().Where(e => IsValid(e.AttributeClass)) )
            {
                var varName = $"component_{symbol.AttributeClass!.Name}";

                if (Bindings.Contains(varName))
                {
                    continue;
                }

                Components.AppendLine($"private Property.Binding {varName};");
                builder.AppendLine($"{varName} = {OnBinding(symbol)};\nComponents.Add({varName});");

                Bindings.Add(varName);
            }

            return builder.ToString();
        }

        private string OnBinding(AttributeData attribute)
        {
            var builder = new StringBuilder();

            builder.Append($"new {Factory.OnType(attribute.AttributeClass).Replace("Attribute", "")}(");

            for (var i = 0; i < attribute.ConstructorArguments.Length; i++)
            {
                var argument = attribute.ConstructorArguments[i];
                var arg = argument.Value;

                // This is aids...
                if (argument.Type!.Name.Equals("string", StringComparison.OrdinalIgnoreCase))
                {
                    arg = $@"""{arg}""";
                }
                
                if (argument.Type!.Name.Equals("boolean", StringComparison.OrdinalIgnoreCase))
                {
                    arg = (bool)arg ? "true" : "false";
                }

                builder.Append(arg);

                if (i != attribute.ConstructorArguments.Length - 1)
                {
                    builder.Append(", ");
                }
            }

            builder.Append("){");

            foreach ( var args in attribute.NamedArguments )
            {
                var arg = args.Value.Value;

                // This is aids...
                if (args.Value.Type.Name.Equals("string", StringComparison.OrdinalIgnoreCase))
                {
                    arg = $@"""{arg}""";
                }
                
                if (args.Value.Type!.Name.Equals("boolean", StringComparison.OrdinalIgnoreCase))
                {
                    arg = (bool)arg ? "true" : "false";
                }

                builder.AppendLine($"{args.Key} = {arg},");
            }

            return builder.Append('}').ToString();
        }

        // Property

        public string Owner { get; }

        private string OnGetter()
        {
            if (Symbol.GetMethod == null || Symbol.GetMethod.DeclaredAccessibility == Accessibility.Private)
                return @"value = default;";

            return $@"value = {(Symbol.IsStatic ? $"{Owner}.{Symbol.Name}" : $"(({Owner})from).{Symbol.Name}")};";
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
