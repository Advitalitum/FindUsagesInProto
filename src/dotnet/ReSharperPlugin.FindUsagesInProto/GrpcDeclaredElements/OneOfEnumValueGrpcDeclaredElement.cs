using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.Util;
using ReSharperPlugin.FindUsagesInProto.Helpers;

namespace ReSharperPlugin.FindUsagesInProto;

public class OneOfEnumValueGrpcDeclaredElement : ClassOrConstructorGrpcDeclaredElement
{
    private OneOfEnumValueGrpcDeclaredElement(IClass classDeclaration) : base(classDeclaration)
    {
    }

    public static bool TryCreate(IClrDeclaredElement clrDeclaredElement, out OneOfEnumValueGrpcDeclaredElement element)
    {
        switch (clrDeclaredElement)
        {
            case IField field
                when field.IsEnumMember
                     && field.ContainingType is IEnum @enum
                     && @enum.ContainingType is IClass classDeclaration
                     && classDeclaration.IsGrpcGeneratedClass()
                     && @enum.ShortName.EndsWith("OneofCase")
                     && classDeclaration
                         .Properties
                         .OfType<ICSharpProperty>()
                         .FirstOrDefault(p => p.ShortName == field.ShortName)
                         ?.Type
                         .GetTypeElement() is IClass oneOfPropertyClassDeclaration:

                element = new OneOfEnumValueGrpcDeclaredElement(oneOfPropertyClassDeclaration);
                return true;

            default:

                element = null;
                return false;
        }
    }
}