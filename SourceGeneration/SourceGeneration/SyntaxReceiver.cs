using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGeneration
{
    public class SyntaxReceiver : ISyntaxContextReceiver
    {
        public List<INamedTypeSymbol> Properties { get; } = new List<INamedTypeSymbol>();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (!(context.Node is ClassDeclarationSyntax classDeclarationSyntax) ||
                classDeclarationSyntax.AttributeLists.Count <= 0) return;

            if (context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax) is INamedTypeSymbol classSymbol && 
                classSymbol.GetAttributes().Any(ad => ad.AttributeClass != null && 
                                                      ad.AttributeClass.ToDisplayString() == "SourceGeneration.DataModel.DataModelAttribute"))
            {
                Properties.Add(classSymbol);
            }
        }
    }
}