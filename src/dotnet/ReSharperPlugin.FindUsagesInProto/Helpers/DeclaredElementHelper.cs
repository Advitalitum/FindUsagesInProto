using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Protobuf;
using JetBrains.ReSharper.Psi.Web.WebConfig;

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

    public static IEnumerable<IPsiSourceFile> GetSuitableProtoFiles(this IDeclaredElement declaredElement,
        GrpcCsharpDeclaredElement grpcDeclaredElement)
    {
        var psiServices = declaredElement.GetPsiServices();

        return psiServices
            .Solution
            .GetAllProjects()
            .SelectMany(x => x.GetAllProjectFiles(WithProtobufFileType))
            .Select(y => y.ToSourceFile())
            .Where(x => x is not null)
            .Where(x => psiServices.WordIndex.CanContainAllSubwords(x, grpcDeclaredElement.ShortName));
    }

    private static bool WithProtobufFileType(IProjectFile projectFile)
    {
        return projectFile.LanguageType is ProtobufProjectFileType;
    }
}