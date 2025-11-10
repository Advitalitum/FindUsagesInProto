using System.Text.RegularExpressions;

namespace ReSharperPlugin.FindUsagesInProto;

public abstract class GrpcCsharpDeclaredElement : IDeclaredElementInfo
{
    public abstract string ShortName { get; }

    public abstract Regex GetRegexForSearchInText();
}