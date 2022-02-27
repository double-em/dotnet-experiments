using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGeneration
{
    public class SyntaxReceiver : ISyntaxContextReceiver
    {
        public List<IPropertySymbol> Properties { get; } = new List<IPropertySymbol>();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax classDeclarationSyntax &&
                classDeclarationSyntax.AttributeLists.Count > 0)
            {
                foreach (var namedTypeSymbol in classDeclarationSyntax.Members)
                {
                    var declaredSymbol = context.SemanticModel.GetDeclaredSymbol(namedTypeSymbol);
                    if (declaredSymbol is IPropertySymbol classSymbol)
                    {
                        Properties.Add(classSymbol);
                    } 
                }
            }
        }
    }
}