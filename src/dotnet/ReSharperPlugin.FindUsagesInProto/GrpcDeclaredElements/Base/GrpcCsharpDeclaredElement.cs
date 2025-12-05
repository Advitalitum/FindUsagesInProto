using JetBrains.Annotations;

namespace ReSharperPlugin.FindUsagesInProto;

public abstract class GrpcCsharpDeclaredElement : IDeclaredElementInfo
{
    [NotNull]
    public abstract string ShortName { get; }

   [NotNull]
    public abstract GrpcElementSearchInfo GetSearchInfo();
}