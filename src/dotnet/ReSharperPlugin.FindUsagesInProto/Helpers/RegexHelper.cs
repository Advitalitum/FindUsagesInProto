using System.Text.RegularExpressions;
using JetBrains.ReSharper.Psi;

namespace ReSharperPlugin.FindUsagesInProto.Helpers;

public static class RegexHelper
{
    public static Regex GetRegex(IClass classDeclaration)
    {
        var namespaceQualifiedName = classDeclaration.GetContainingNamespace().QualifiedName;

        return CreateRegex(namespaceQualifiedName, classDeclaration.ShortName);
    }

    private static Regex CreateRegex(string containingNamespaceQualifiedName, string shortName)
    {
        var namespaceName = containingNamespaceQualifiedName.Replace(".", @"\.");

        return new Regex(
            $$"""csharp_namespace\s*\=\s*\"{{namespaceName}}\"[\s\S]*(message\s+{{shortName}}\s*\{)""",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}