using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Eggshell.Generator
{
    /// <summary>
    /// Creates a function from a Symbol, that is used in Eggshells
    /// compile time reflection system.
    /// </summary>
    public class Function : Member<IMethodSymbol>
    {
        public Function(ISymbol symbol) : base(symbol)
        {
            Owner = Factory.OnType(symbol.ContainingType);
        }

        public override string Compile(out string className)
        {
            className = $"{Owner}.{Symbol.Name}".Replace('.', '_');

            return $@"
[CompilerGenerated]
private class {className} : Function
{{
	public {className}() : base( ""{Name}"", ""{Symbol.Name}"" )
	{{
		Title = ""{Title}"";
		Group = ""{Group}"";
		Help = @""{Help}"";
		IsStatic = {(Symbol.IsStatic ? "true" : "false")};

        {OnParameters()}
	}}

    public override void OnAttached(Library library)
    {{
        // Add Components
		{OnComponents()}
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

                Components.AppendLine($"private Function.Binding {varName};");
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

        protected string OnParameters()
        {
            if (Symbol.Parameters.Length == 0)
            {
                return "Parameters = System.Array.Empty<System.Type>();";
            }
            
            var builder = new StringBuilder("Parameters = new System.Type[] {");

            foreach ( var parameter in Symbol.Parameters )
            {
                builder.Append($"typeof({Factory.OnType(parameter.Type)}),");
            }

            return builder.Append("};").ToString();
        }

        // Property

        public string Owner { get; }

        // Static

        public static bool IsValid(ISymbol symbol, ITypeSymbol typeSymbol)
        {
            return symbol.Kind == SymbolKind.Method
                   && !symbol.IsOverride
                   && !symbol.Name.StartsWith(".ctor")
                   && !symbol.Name.StartsWith("op_")
                   && !symbol.Name.StartsWith("get_")
                   && !symbol.Name.StartsWith("set_")
                   && symbol.ContainingType.Equals(typeSymbol, SymbolEqualityComparer.Default)
                   && symbol.GetAttributes().Any(e => e.AttributeClass!.AllInterfaces.Any(e => e.Name.StartsWith("IComponent"))) ||
                   symbol.GetAttributes().Any(attribute => attribute.AttributeClass!.Name.StartsWith("Function"));
        }
    }
}
