using System.Text.RegularExpressions;
using JetBrains.ReSharper.Psi;

namespace ReSharperPlugin.FindUsagesInProto;

public record GrpcElementSearchInfo
{
    private readonly string _elementSearchPattern;
    private readonly string _csharpNamespaceQualifiedNameFixed;

    public GrpcElementSearchInfo(string elementSearchPattern, INamespace csharpNamespace)
    {
        _elementSearchPattern = elementSearchPattern;
        _csharpNamespaceQualifiedNameFixed = csharpNamespace.QualifiedName.Replace(".", @"\.");
    }

    public Regex SearchWithCsharpNamespaceRegex =>
        new($"""csharp_namespace\s*\=\s*\"{_csharpNamespaceQualifiedNameFixed}\"[\s\S]*{_elementSearchPattern}""",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public Regex SearchWithoutCsharpNamespaceRegex => new(_elementSearchPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
}