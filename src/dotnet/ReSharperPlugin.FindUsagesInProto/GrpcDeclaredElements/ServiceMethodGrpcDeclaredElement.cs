using System.Linq;
using JetBrains.ReSharper.Psi;
using ReSharperPlugin.FindUsagesInProto.Helpers;

namespace ReSharperPlugin.FindUsagesInProto;

public class ServiceMethodGrpcDeclaredElement : GrpcCsharpDeclaredElement
{
    private readonly IClass _rootGrpcClass;
    private readonly IMethod _method;

    private ServiceMethodGrpcDeclaredElement(IMethod method, IClass rootGrpcClass)
    {
        _rootGrpcClass = rootGrpcClass;
        _method = method;
    }

    public static bool TryCreate(IClrDeclaredElement clrDeclaredElement, out ServiceMethodGrpcDeclaredElement element)
    {
        switch (clrDeclaredElement)
        {
            case IMethod method when method.IsOverride
                                     && method.ContainingType is IClass methodContainingClass
                                     && methodContainingClass.GetSuperClass()?.GetContainingType() is IClass rootGrpcClass
                                     && method.GetAllSuperMembers()
                                         .Where(x => method.OverridesOrImplements(x.Member))
                                         .Any(x => x.Element.GetAttributeInstances(true)
                                             .Any(a => a.GetAttributeShortName() == "GeneratedCodeAttribute"
                                                       && a.PositionParameters().Any(p =>
                                                           p.ConstantValue.StringValue == "grpc_csharp_plugin")
                                             )
                                         ):

                element = new ServiceMethodGrpcDeclaredElement(method, rootGrpcClass);
                return true;

            default:

                element = null;
                return false;
        }
    }

    public override string ShortName => _method.ShortName;

    public override GrpcElementSearchInfo GetSearchInfo() =>
        $$"""service\s+{{_rootGrpcClass.ShortName}}\s*\{[\s\S]*rpc\s+({{_method.ShortName}})\s*\([^\}]*}"""
            .ToGrpcElementSearchHelper(_rootGrpcClass.GetContainingNamespace());
}