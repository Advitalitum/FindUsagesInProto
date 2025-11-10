namespace ReSharperPlugin.FindUsagesInProto;

public sealed class NonGrpcCsharpDeclaredElement : IDeclaredElementInfo
{
    public static readonly NonGrpcCsharpDeclaredElement Instance = new();

    private NonGrpcCsharpDeclaredElement()
    {
    }
}