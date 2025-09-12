using System.Threading;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NUnit.Framework;

[assembly: Apartment(ApartmentState.STA)]

namespace ReSharperPlugin.FindUsagesInProto.Tests
{
    [ZoneDefinition]
    public class FindUsagesInProtoTestEnvironmentZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>, IRequire<IFindUsagesInProtoZone> { }

    [ZoneMarker]
    public class ZoneMarker : IRequire<ICodeEditingZone>, IRequire<ILanguageCSharpZone>, IRequire<FindUsagesInProtoTestEnvironmentZone> { }

    [SetUpFixture]
    public class FindUsagesInProtoTestsAssembly : ExtensionTestEnvironmentAssembly<FindUsagesInProtoTestEnvironmentZone> { }
}
