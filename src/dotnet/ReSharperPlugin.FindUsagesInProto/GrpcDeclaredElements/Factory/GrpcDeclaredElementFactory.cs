using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace ReSharperPlugin.FindUsagesInProto;

public static class GrpcDeclaredElementFactory
{
    [ContractAnnotation("=> true, element:notnull; => false, element:null")]
    public static bool TryCreate(IClrDeclaredElement clrDeclaredElement,
        [CanBeNull] out GrpcCsharpDeclaredElement element)
    {
        if (ClassOrConstructorGrpcDeclaredElement.TryCreate(clrDeclaredElement,
                out var classOrConstructorGrpcDeclaredElement))
        {
            element = classOrConstructorGrpcDeclaredElement;
            return true;
        }

        if (OneOfPropertyGrpcDeclaredElement.TryCreate(clrDeclaredElement, out var oneOfPropertyGrpcDeclaredElement))
        {
            element = oneOfPropertyGrpcDeclaredElement;
            return true;
        }

        if (PropertyGrpcDeclaredElement.TryCreate(clrDeclaredElement, out var propertyGrpcDeclaredElement))
        {
            element = propertyGrpcDeclaredElement;
            return true;
        }

        if (OneOfEnumValueGrpcDeclaredElement.TryCreate(clrDeclaredElement, out var oneOfEnumValueGrpcDeclaredElement))
        {
            element = oneOfEnumValueGrpcDeclaredElement;
            return true;
        }

        if (EnumValueGrpcDeclaredElement.TryCreate(clrDeclaredElement, out var enumValueGrpcDeclaredElement))
        {
            element = enumValueGrpcDeclaredElement;
            return true;
        }

        if (OneOfEnumTypeGrpcDeclaredElement.TryCreate(clrDeclaredElement, out var oneOfEnumTypeGrpcDeclaredElement))
        {
            element = oneOfEnumTypeGrpcDeclaredElement;
            return true;
        }

        if (EnumTypeGrpcDeclaredElement.TryCreate(clrDeclaredElement, out var enumTypeGrpcDeclaredElement))
        {
            element = enumTypeGrpcDeclaredElement;
            return true;
        }

        if (ServiceMethodGrpcDeclaredElement.TryCreate(clrDeclaredElement, out var serviceMethodGrpcDeclaredElement))
        {
            element = serviceMethodGrpcDeclaredElement;
            return true;
        }

        if (ClientMethodGrpcDeclaredElement.TryCreate(clrDeclaredElement, out var clientMethodGrpcDeclaredElement))
        {
            element = clientMethodGrpcDeclaredElement;
            return true;
        }

        element = null;
        return false;
    }
}