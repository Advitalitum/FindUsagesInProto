using System.Linq;
using JetBrains.ReSharper.Psi;

namespace ReSharperPlugin.FindUsagesInProto;

public class EnumValueGrpcDeclaredElement : GrpcCsharpDeclaredElement
{
    private readonly IEnum _enum;
    private readonly IField _field;

    private EnumValueGrpcDeclaredElement(IField field, IEnum @enum)
    {
        _enum = @enum;
        _field = field;
    }

    public static bool TryCreate(IClrDeclaredElement clrDeclaredElement, out EnumValueGrpcDeclaredElement element)
    {
        switch (clrDeclaredElement)
        {
            case IField field
                when field.IsEnumMember
                     && field.ContainingType is IEnum @enum
                     && field.GetAttributeInstances(true)
                         .Any(a => a.GetAttributeShortName() == "OriginalNameAttribute" && a.PositionParameterCount == 1):

                element = new EnumValueGrpcDeclaredElement(field, @enum);
                return true;

            default:

                element = null;
                return false;
        }
    }

    public override string ShortName => GetEnumValueGrpcName();
    
    protected override INamespace CsharpNamespace => _enum.GetContainingNamespace();

    protected override string GetElementSearchPattern() =>  $$"""enum\s+{{_enum.ShortName}}\s*\{[^\}]*\W({{GetEnumValueGrpcName()}})\W[^\}]*}""";

    private string GetEnumValueGrpcName()
    {
        return _field
            .GetAttributeInstances(true)
            .First(a => a.GetAttributeShortName() == "OriginalNameAttribute" && a.PositionParameterCount == 1)
            .PositionParameters()
            .Single()
            .ConstantValue.StringValue;
    }
}