using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
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

    public override Regex GetRegexForSearchInText()
    {
        var namespaceQualifiedName = _rootGrpcClass.GetContainingNamespace().QualifiedName;

        var namespaceName = namespaceQualifiedName.Replace(".", @"\.");
        var returnTypeName = _method.ReturnType.GetPresentableName(_method.PresentationLanguage);
        var isAsyncMethod = returnTypeName.Contains("AsyncUnaryCall");
        var methodNameInProto = isAsyncMethod ? _method.ShortName.RemoveEnd("Async") : _method.ShortName;
        
        return new Regex(
            $$"""csharp_namespace\s*\=\s*\"{{namespaceName}}\"[\s\S]*service\s+{{_rootGrpcClass.ShortName}}\s*\{[\s\S]*(rpc\s+{{methodNameInProto}}\s*\()[^\}]*}""",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}