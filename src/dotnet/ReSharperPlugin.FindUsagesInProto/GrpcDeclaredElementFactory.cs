using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace ReSharperPlugin.FindUsagesInProto;

public static class GrpcDeclaredElementFactory
{
    [ContractAnnotation("=> true, element:notnull; => false, element:null")]
    public static bool TryCreate(IClrDeclaredElement declaredElement, [CanBeNull] out GrpcCsharpDeclaredElement element)
    {
        if (ClassOrConstructorGrpcDeclaredElement.TryCreate(declaredElement,
                out var classOrConstructorGrpcDeclaredElement))
        {
            element = classOrConstructorGrpcDeclaredElement;
            return true;
        }

        if (PropertyGrpcDeclaredElement.TryCreate(declaredElement, out var propertyGrpcDeclaredElement))
        {
            element = propertyGrpcDeclaredElement;
            return true;
        }

        if (EnumValueGrpcDeclaredElement.TryCreate(declaredElement, out var enumValueGrpcDeclaredElement))
        {
            element = enumValueGrpcDeclaredElement;
            return true;
        }

        if (EnumTypeGrpcDeclaredElement.TryCreate(declaredElement, out var enumTypeGrpcDeclaredElement))
        {
            element = enumTypeGrpcDeclaredElement;
            return true;
        }

        element = null;
        return false;
    }
}