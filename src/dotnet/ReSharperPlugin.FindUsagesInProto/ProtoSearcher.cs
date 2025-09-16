using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.DocumentManagers;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Search;

namespace ReSharperPlugin.FindUsagesInProto;

[PsiComponent(Instantiation.DemandAnyThreadSafe)]
public class ProtoSearcher : DomainSpecificSearcherFactoryBase
{
    private static readonly ConcurrentDictionary<(string NamespaceQualifiedName, string ShortName), Regex> Regexes =
        new();

    public override bool IsCompatibleWithLanguage(PsiLanguageType languageType)
    {
        return IsCsharpLanguage(languageType);
    }

    public override IEnumerable<FindResult> GetRelatedFindResults(IDeclaredElement element)
    {
        var grpcClassDeclaration = GetGrpcClassDeclarationOrNull(element);

        if (grpcClassDeclaration is null)
        {
            return [];
        }

        var regex = GetOrCreateRegex(grpcClassDeclaration);

        var results = grpcClassDeclaration
            .GetSolution()
            .GetAllProjects()
            .SelectMany(x => x.GetAllProjectFiles(WithProtoExtension))
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
        if (IsCsharpLanguage(element.PresentationLanguage) is false ||
            element is not IClrDeclaredElement clrDeclaredElement)
        {
            return null;
        }

        var classDeclaration = ExtractClassDeclarationFromElementOrNull(clrDeclaredElement);

        if (classDeclaration is null)
        {
            return null;
        }

        return IsGrpcGeneratedClass(classDeclaration) ? classDeclaration : null;
    }

    private static bool IsCsharpLanguage(PsiLanguageType elementPresentationLanguage)
    {
        return elementPresentationLanguage is CSharpLanguage;
    }

    private static Regex GetOrCreateRegex(IClass classDeclaration)
    {
        var namespaceQualifiedName = classDeclaration.GetContainingNamespace().QualifiedName;

        return Regexes.GetOrAdd((namespaceQualifiedName, classDeclaration.ShortName),
            x => CreateRegex(x.NamespaceQualifiedName, x.ShortName));
    }

    private static Regex CreateRegex(string containingNamespaceQualifiedName, string shortName)
    {
        var namespaceName = containingNamespaceQualifiedName.Replace(".", @"\.");

        return new Regex(
            $$"""csharp_namespace\s*\=\s*\"{{namespaceName}}\"[\s\S]*(message\s+{{shortName}}\s*\{)""",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    private static bool WithProtoExtension(IProjectFile y)
    {
        return y.Name.EndsWith(".proto", StringComparison.OrdinalIgnoreCase);
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

    private static bool IsGrpcGeneratedClass(IClass classDeclaration)
    {
        return classDeclaration.IsAbstract is false
               && classDeclaration.IsSealed
               && ConstructorHasGrpcGeneratedCodeAttribute(classDeclaration);
    }

    private static bool ConstructorHasGrpcGeneratedCodeAttribute(IClass classDeclaration)
    {
        return classDeclaration.Constructors.Any(c => c.GetAttributeInstances(true).Any(a =>
            a.GetAttributeShortName() == "GeneratedCodeAttribute" &&
            a.PositionParameters().Any(p => p.ConstantValue.StringValue == "protoc")));
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