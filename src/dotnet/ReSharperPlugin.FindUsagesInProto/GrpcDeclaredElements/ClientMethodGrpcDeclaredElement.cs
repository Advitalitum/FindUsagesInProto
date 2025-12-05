using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.Util.Extension;

namespace ReSharperPlugin.FindUsagesInProto;

public class ClientMethodGrpcDeclaredElement : GrpcCsharpDeclaredElement
{
    private readonly IClass _rootGrpcClass;
    private readonly IMethod _method;

    private ClientMethodGrpcDeclaredElement(IMethod method, IClass rootGrpcClass)
    {
        _rootGrpcClass = rootGrpcClass;
        _method = method;
    }

    public static bool TryCreate(IClrDeclaredElement clrDeclaredElement, out ClientMethodGrpcDeclaredElement element)
    {
        switch (clrDeclaredElement)
        {
            case IMethod method when method.IsVirtual
                                     && method.ContainingType?.GetContainingType() is IClass rootGrpcClass
                                     && method.GetAttributeInstances(true)
                                             .Any(a => a.GetAttributeShortName() == "GeneratedCodeAttribute"
                                                       && a.PositionParameters().Any(p =>
                                                           p.ConstantValue.StringValue == "grpc_csharp_plugin")
                                             ):

                element = new ClientMethodGrpcDeclaredElement(method, rootGrpcClass);
                return true;

            default:

                element = null;
                return false;
        }
    }

    public override string ShortName => _method.ShortName;
    
    protected override INamespace CsharpNamespace => _rootGrpcClass.GetContainingNamespace();

    protected override string GetElementSearchPattern()
    {
        var returnTypeName = _method.ReturnType.GetPresentableName(_method.PresentationLanguage);
        var isAsyncMethod = returnTypeName.Contains("AsyncUnaryCall");
        var methodNameInProto = isAsyncMethod ? _method.ShortName.RemoveEnd("Async") : _method.ShortName;

        return $$"""service\s+{{_rootGrpcClass.ShortName}}\s*\{[\s\S]*rpc\s+({{methodNameInProto}})\s*\([^\}]*}""";
    }
}