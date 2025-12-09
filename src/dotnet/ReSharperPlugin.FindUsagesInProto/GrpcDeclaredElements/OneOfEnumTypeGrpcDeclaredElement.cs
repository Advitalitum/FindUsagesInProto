using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.Util;
using ReSharperPlugin.FindUsagesInProto.Helpers;

namespace ReSharperPlugin.FindUsagesInProto;

public class OneOfEnumTypeGrpcDeclaredElement : ClassOrConstructorGrpcDeclaredElement
{
    private OneOfEnumTypeGrpcDeclaredElement(IClass classDeclaration) : base(classDeclaration)
    {
    }

    public static bool TryCreate(IClrDeclaredElement clrDeclaredElement, out OneOfEnumTypeGrpcDeclaredElement element)
    {
        switch (clrDeclaredElement)
        {
            case IEnum @enum
                when @enum.ContainingType is IClass classDeclaration
                     && classDeclaration.IsGrpcGeneratedClass()
                     && @enum.ShortName.EndsWith("OneofCase")
                     && classDeclaration
                         .Properties
                         .OfType<ICSharpProperty>()
                         .Any(p => @enum.Equals(p.Type.GetTypeElement())):

                element = new OneOfEnumTypeGrpcDeclaredElement(classDeclaration);
                return true;

            default:

                element = null;
                return false;
        }
    }
}