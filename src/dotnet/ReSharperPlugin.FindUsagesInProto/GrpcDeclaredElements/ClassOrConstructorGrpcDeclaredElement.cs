using JetBrains.ReSharper.Psi;
using ReSharperPlugin.FindUsagesInProto.Helpers;

namespace ReSharperPlugin.FindUsagesInProto;

public class ClassOrConstructorGrpcDeclaredElement : GrpcCsharpDeclaredElement
{
    private readonly IClass _classDeclaration;

    protected ClassOrConstructorGrpcDeclaredElement(IClass classDeclaration)
    {
        _classDeclaration = classDeclaration;
    }

    public static bool TryCreate(IClrDeclaredElement clrDeclaredElement,
        out ClassOrConstructorGrpcDeclaredElement element)
    {
        switch (clrDeclaredElement)
        {
            case IClass classDeclaration when classDeclaration.IsGrpcGeneratedClass():

                element = new ClassOrConstructorGrpcDeclaredElement(classDeclaration);
                return true;

            case IConstructor constructor when
                constructor.ContainingType is IClass constructorClassDeclaration
                && constructorClassDeclaration.IsGrpcGeneratedClass():

                element = new ClassOrConstructorGrpcDeclaredElement(constructorClassDeclaration);
                return true;

            default:

                element = null;
                return false;
        }
    }

    public override string ShortName => _classDeclaration.ShortName;

    protected override INamespace CsharpNamespace => _classDeclaration.GetContainingNamespace();

    protected override string GetElementSearchPattern() => $$"""message\s+({{ShortName}})\s*\{""";
}