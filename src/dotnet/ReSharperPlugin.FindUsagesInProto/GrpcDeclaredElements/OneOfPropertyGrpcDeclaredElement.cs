using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.Util;
using ReSharperPlugin.FindUsagesInProto.Helpers;

namespace ReSharperPlugin.FindUsagesInProto;

public class OneOfPropertyGrpcDeclaredElement : ClassOrConstructorGrpcDeclaredElement
{
    private OneOfPropertyGrpcDeclaredElement(IClass classDeclaration) : base(classDeclaration)
    {
    }

    public static bool TryCreate(IClrDeclaredElement clrDeclaredElement, out OneOfPropertyGrpcDeclaredElement element)
    {
        switch (clrDeclaredElement)
        {
            case ICSharpProperty property
                when property.ContainingType is IClass propertyContainingClassDeclaration
                     && propertyContainingClassDeclaration.IsGrpcGeneratedClass()
                     && property.Type.GetTypeElement() is IEnum @enum
                     && @enum.ContainingType is IClass enumContainingClassDeclaration
                     && enumContainingClassDeclaration.Equals(property.ContainingType)
                     && @enum.ShortName.EndsWith("OneofCase")
                :

                element = new OneOfPropertyGrpcDeclaredElement(propertyContainingClassDeclaration);
                return true;

            default:

                element = null;
                return false;
        }
    }
}