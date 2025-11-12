using System.Text.RegularExpressions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using ReSharperPlugin.FindUsagesInProto.Helpers;

namespace ReSharperPlugin.FindUsagesInProto;

public class PropertyGrpcDeclaredElement : GrpcCsharpDeclaredElement
{
    private readonly IClass _classDeclaration;
    private readonly ICSharpProperty _property;

    private PropertyGrpcDeclaredElement(ICSharpProperty property, IClass classDeclaration)
    {
        _classDeclaration = classDeclaration;
        _property = property;
    }

    public static bool TryCreate(IClrDeclaredElement clrDeclaredElement, out PropertyGrpcDeclaredElement element)
    {
        switch (clrDeclaredElement)
        {
            case ICSharpProperty property when property.ContainingType is IClass classDeclaration &&
                                               classDeclaration.IsGrpcGeneratedClass():

                element = new PropertyGrpcDeclaredElement(property, classDeclaration);
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

        var namespaceName = namespaceQualifiedName.Replace(".", @"\.");

        return new Regex(
            $$"""csharp_namespace\s*\=\s*\"{{namespaceName}}\"[\s\S]*message\s+{{_classDeclaration.ShortName}}\s*\{[^\}]*\W({{_property.ShortName.Underscore()}})\W[^\}]*}""",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}