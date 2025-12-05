using JetBrains.ReSharper.Psi;

namespace ReSharperPlugin.FindUsagesInProto;

public class EnumTypeGrpcDeclaredElement : GrpcCsharpDeclaredElement
{
    private readonly IEnum _enum;

    private EnumTypeGrpcDeclaredElement(IEnum @enum)
    {
        _enum = @enum;
    }

    public static bool TryCreate(IClrDeclaredElement clrDeclaredElement, out EnumTypeGrpcDeclaredElement element)
    {
        switch (clrDeclaredElement)
        {
            case IEnum @enum:

                element = new EnumTypeGrpcDeclaredElement(@enum);
                return true;

            default:

                element = null;
                return false;
        }
    }

    public override string ShortName => _enum.ShortName;
    
    protected override INamespace CsharpNamespace => _enum.GetContainingNamespace();

    protected override string GetElementSearchPattern() => $$"""enum\s+({{_enum.ShortName}})\s*\{""";
}