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
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver))
                return;

            INamedTypeSymbol attributeSymbol = context.Compilation.GetTypeByMetadataName("SourceGeneration.DataModel.DataModelAttribute");

            foreach (IGrouping<INamedTypeSymbol, IPropertySymbol> group in receiver.Properties.GroupBy(f => f.ContainingType))
            {
                string classSource = ProcessClass(group.Key, group.ToList(), attributeSymbol, context);
                context.AddSource($"{group.Key.Name}.g.cs", SourceText.From(classSource, Encoding.UTF8));
            }
        }

        private string ProcessClass(INamedTypeSymbol classSymbol, List<IPropertySymbol> fields, ISymbol attributeSymbol, GeneratorExecutionContext context)
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

        private void ProcessField(StringBuilder source, IPropertySymbol propertySymbol, ISymbol attributeSymbol)
        {
            var propertyName = propertySymbol.Name;
            var propertyType = propertySymbol.Type;

            source.Append($@"
                public {propertyType} {propertyName}
                {{
                    get
                    {{
                        return this.{propertyName};
                    }}
                    
                    set
                    {{
                        this.{propertyName} = value;
                    }}
                }}

                ");
        }
    }
}