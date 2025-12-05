using System.Text.RegularExpressions;
using JetBrains.ReSharper.Psi;
using ReSharperPlugin.FindUsagesInProto.Helpers;

namespace ReSharperPlugin.FindUsagesInProto;

public class ClassOrConstructorGrpcDeclaredElement : GrpcCsharpDeclaredElement
{
    private readonly IClass _classDeclaration;

    private ClassOrConstructorGrpcDeclaredElement(IClass classDeclaration)
    {
        _classDeclaration = classDeclaration;
    }

    public static bool TryCreate(IClrDeclaredElement clrDeclaredElement, out ClassOrConstructorGrpcDeclaredElement element)
    {
        switch (clrDeclaredElement)
        {
            case IClass classDeclaration when classDeclaration.IsGrpcGeneratedClass():

                element = new ClassOrConstructorGrpcDeclaredElement(classDeclaration);
                return true;

            case IConstructor constructor when
                constructor.ContainingType is IClass constructorClassDeclaration
                && constructorClassDeclaration.IsGrpcGeneratedClass():

                element = new ClassOrConstructorGrpcDeclaredElement(constructorClassDeclaration);
                return true;

            default:

                element = null;
                return false;
        }
    }
    
    public override string ShortName => _classDeclaration.ShortName;

    public override Regex GetRegexForSearchInText()
    {
        var namespaceQualifiedName = _classDeclaration.GetContainingNamespace().QualifiedName;

        return CreateRegex(namespaceQualifiedName, _classDeclaration.ShortName);
    }

    private static Regex CreateRegex(string containingNamespaceQualifiedName, string shortName)
    {
        var namespaceName = containingNamespaceQualifiedName.Replace(".", @"\.");

        return new Regex(
            $$"""csharp_namespace\s*\=\s*\"{{namespaceName}}\"[\s\S]*message\s+({{shortName}})\s*\{""",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}