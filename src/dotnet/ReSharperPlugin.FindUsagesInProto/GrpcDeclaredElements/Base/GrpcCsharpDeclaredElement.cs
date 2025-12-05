using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace ReSharperPlugin.FindUsagesInProto;

public abstract class GrpcCsharpDeclaredElement : IDeclaredElementInfo
{
    [NotNull]
    public abstract string ShortName { get; }

    [NotNull]
    public GrpcElementSearchInfo GetSearchInfo() => new(GetElementSearchPattern(), CsharpNamespace);

    [NotNull]
    protected abstract INamespace CsharpNamespace { get; }

    [NotNull]
    protected abstract string GetElementSearchPattern();
}