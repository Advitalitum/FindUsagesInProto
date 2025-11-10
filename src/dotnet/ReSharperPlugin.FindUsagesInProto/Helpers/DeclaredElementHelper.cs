using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;

namespace ReSharperPlugin.FindUsagesInProto.Helpers;

public static class DeclaredElementHelper
{
    public static bool IsCsharpLanguage(this PsiLanguageType elementPresentationLanguage)
    {
        return elementPresentationLanguage is CSharpLanguage;
    }

    public static bool IsGrpcGeneratedClass(this IClass classDeclaration)
    {
        return classDeclaration.IsAbstract is false
               && classDeclaration.IsSealed
               && ConstructorHasGrpcGeneratedCodeAttribute(classDeclaration);
    }

    private static bool ConstructorHasGrpcGeneratedCodeAttribute(IClass classDeclaration)
    {
        return classDeclaration.Constructors.Any(c =>
            c.GetAttributeInstances(true)
                .Any(a =>
                    a.GetAttributeShortName() == "GeneratedCodeAttribute" &&
                    a.PositionParameters().Any(p => p.ConstantValue.StringValue == "protoc")
                )
        );
    }

    public static IEnumerable<IProjectFile> GetAllSolutionProtoFiles(this IDeclaredElement declaredElement)
    {
        return declaredElement
            .GetSolution()
            .GetAllProjects()
            .SelectMany(x => x.GetAllProjectFiles(WithProtoExtension));
    }

    private static bool WithProtoExtension(IProjectFile projectFile)
    {
        return projectFile.Name.EndsWith(".proto", StringComparison.OrdinalIgnoreCase);
    }
}