using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.DocumentManagers;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
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

        var regex = grpcDeclaredElement.GetRegexForSearchInText();

        var results = element
            .GetAllSolutionProtoFiles()
            .Select(x => MapResultIfMatch(regex, x))
            .Where(x => x is not null);

        return results;
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
    private static FindResultText MapResultIfMatch(Regex regex, IProjectFile projectFile)
    {
        var document = projectFile.GetDocument();
        var match = regex.Match(document.GetText());

        if (match.Success is false)
        {
            return null;
        }

        return new FindResultText(projectFile.ToSourceFile(), new DocumentRange(document, match.Groups[1].Index));
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