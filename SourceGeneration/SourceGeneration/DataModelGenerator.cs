using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SourceGeneration
{
    [Generator]
    public class DataModelGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
// #if DEBUG
//             if (!Debugger.IsAttached)
//             {
//                 Debugger.Launch();
//             }
// #endif
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver))
                return;

            INamedTypeSymbol attributeSymbol = context.Compilation.GetTypeByMetadataName("SourceGeneration.DataModel.DataModelAttribute") ?? throw new ArgumentNullException("context.Compilation.GetTypeByMetadataName(\"SourceGeneration.DataModel.DataModelAttribute\")");

            foreach (IGrouping<INamedTypeSymbol, INamedTypeSymbol> group in receiver.Properties.GroupBy(f => f.ContainingType))
            {
                string classSource = ProcessClass(group.Key, group.ToList(), attributeSymbol, context);
                context.AddSource($"{group.Key.Name}.g.cs", SourceText.From(classSource, Encoding.UTF8));
            }
        }

        private string ProcessClass(INamedTypeSymbol classSymbol, List<INamedTypeSymbol> fields, INamedTypeSymbol attributeSymbol, GeneratorExecutionContext context)
        {
            if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
            {
                return null;
            }

            var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            var source = new StringBuilder($@"
                namespace {namespaceName}
                {{
                    public partial class {classSymbol.Name}
                    {{");

            foreach (var fieldSymbol in fields)
            {
                ProcessField(source, fieldSymbol, attributeSymbol);
            }

            source.Append("} }");
            return source.ToString();
        }

        private void ProcessField(StringBuilder source, ITypeSymbol fieldSymbol, INamedTypeSymbol attributeSymbol)
        {
            string fieldName = fieldSymbol.Name;
            ITypeSymbol fieldType = fieldSymbol;

            AttributeData attributeData = fieldSymbol.GetAttributes().Single(ad =>
                ad.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default));
            TypedConstant overridenNameOpt =
                attributeData.NamedArguments.SingleOrDefault(kvp => kvp.Key == "PropertyName").Value;

            string propertyName = chooseName(fieldName, overridenNameOpt);
            if (propertyName.Length == 0 || propertyName == fieldName)
            {
                return;
            }
        }

        private string chooseName(string fieldName, TypedConstant overridenNameOpt)
        {
            if (!overridenNameOpt.IsNull)
            {
                return overridenNameOpt.Value.ToString();
            }

            fieldName = fieldName.TrimStart('_');
            if (fieldName.Length == 0)
            {
                return string.Empty;
            }

            if (fieldName.Length == 1)
            {
                return fieldName.ToUpper();
            }

            return fieldName.Substring(0, 1).ToUpper() + fieldName.Substring(1);
        }
    }
}