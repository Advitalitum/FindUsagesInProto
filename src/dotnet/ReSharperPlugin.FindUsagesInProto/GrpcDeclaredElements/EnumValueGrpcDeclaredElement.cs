using System.Linq;
using JetBrains.ReSharper.Psi;
using ReSharperPlugin.FindUsagesInProto.Helpers;

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

    public override GrpcElementSearchInfo GetSearchInfo()
    {
        var enumValueGrpcName = GetEnumValueGrpcName();

        return $$"""enum\s+{{_enum.ShortName}}\s*\{[^\}]*\W({{enumValueGrpcName}})\W[^\}]*}"""
            .ToGrpcElementSearchHelper(_enum.GetContainingNamespace());
    }

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