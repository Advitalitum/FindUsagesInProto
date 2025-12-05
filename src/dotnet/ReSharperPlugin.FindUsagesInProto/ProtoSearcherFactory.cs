using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Search;
using ReSharperPlugin.FindUsagesInProto.Helpers;

namespace ReSharperPlugin.FindUsagesInProto;

[PsiComponent(Instantiation.DemandAnyThreadSafe)]
public class ProtoSearcherFactory : DomainSpecificSearcherFactoryBase
{
    public override bool IsCompatibleWithLanguage(PsiLanguageType languageType)
    {
        return languageType.IsCsharpLanguage();
    }

    public override IEnumerable<FindResult> GetRelatedFindResults(IDeclaredElement element)
    {
        var declaredElementInfo = GetDeclaredElementInfo(element);

        if (declaredElementInfo is not GrpcCsharpDeclaredElement grpcDeclaredElement)
        {
            return [];
        }

        var suitableProtoFiles = element.GetSuitableProtoFiles(grpcDeclaredElement);

        var regexWithCsharpNamespace = grpcDeclaredElement.GetSearchInfo().SearchWithCsharpNamespaceRegex;
        var searchWithCsharpNamespaceResult = suitableProtoFiles
            .Select(x => MapResultIfMatch(regexWithCsharpNamespace, x))
            .Where(x => x is not null)
            .ToArray();

        if (searchWithCsharpNamespaceResult.Any() is false)
        {
            var regexWithoutCsharpNamespace = grpcDeclaredElement.GetSearchInfo().SearchWithoutCsharpNamespaceRegex;
            return suitableProtoFiles
                .Select(x => MapResultIfMatch(regexWithoutCsharpNamespace, x))
                .Where(x => x is not null);
        }
        
        return searchWithCsharpNamespaceResult;
    }

    public override IEnumerable<string> GetAllPossibleWordsInFile(IDeclaredElement element)
    {
        var declaredElementInfo = GetDeclaredElementInfo(element);

        if (declaredElementInfo is not GrpcCsharpDeclaredElement grpcDeclaredElement)
        {
            return [];
        }

        return [grpcDeclaredElement.ShortName];
    }

    [CanBeNull]
    private static FindResultText MapResultIfMatch(Regex regex, IPsiSourceFile psiSourceFile)
    {
        var document = psiSourceFile.Document;
        
        var match = regex.Match(document.GetText());

        if (match.Success is false)
        {
            return null;
        }

        return new FindResultText(psiSourceFile, new DocumentRange(document, match.Groups[1].Index));
    }

    [NotNull]
    private IDeclaredElementInfo GetDeclaredElementInfo(IDeclaredElement element)
    {
        if (element.PresentationLanguage.IsCsharpLanguage()
            && element is IClrDeclaredElement clrDeclaredElement
            && GrpcDeclaredElementFactory.TryCreate(clrDeclaredElement, out var grpcDeclaredElement))
        {
            return grpcDeclaredElement;
        }

        return NonGrpcCsharpDeclaredElement.Instance;
    }
}