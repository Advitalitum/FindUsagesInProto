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
        var grpcClassDeclaration = GetGrpcClassDeclarationOrNull(element);

        if (grpcClassDeclaration is null)
        {
            return [];
        }

        var regex = RegexHelper.GetRegex(grpcClassDeclaration);

        var results = element
            .GetAllSolutionProtoFiles()
            .Select(x => MapResultIfMatch(regex, x))
            .Where(x => x is not null);

        return results;
    }

    public override IEnumerable<string> GetAllPossibleWordsInFile(IDeclaredElement element)
    {
        var grpcClassDeclaration = GetGrpcClassDeclarationOrNull(element);

        return grpcClassDeclaration is not null ? [grpcClassDeclaration.ShortName] : [];
    }

    private static IClass GetGrpcClassDeclarationOrNull(IDeclaredElement element)
    {
        if (element.PresentationLanguage.IsCsharpLanguage() is false ||
            element is not IClrDeclaredElement clrDeclaredElement)
        {
            return null;
        }

        var classDeclaration = ExtractClassDeclarationFromElementOrNull(clrDeclaredElement);

        if (classDeclaration is null)
        {
            return null;
        }

        return classDeclaration.IsGrpcGeneratedClass() ? classDeclaration : null;
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

    [CanBeNull]
    private static IClass ExtractClassDeclarationFromElementOrNull(IClrDeclaredElement clrDeclaredElement)
    {
        return clrDeclaredElement switch
        {
            IClass classDeclaration => classDeclaration,
            IConstructor constructor => constructor.ContainingType as IClass,
            _ => null
        };
    }
}