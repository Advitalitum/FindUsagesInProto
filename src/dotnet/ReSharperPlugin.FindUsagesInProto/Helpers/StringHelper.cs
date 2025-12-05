using System.Text.RegularExpressions;
using JetBrains.ReSharper.Psi;

namespace ReSharperPlugin.FindUsagesInProto.Helpers;

public static class StringHelper
{
    public static string Underscore(this string input)
    {
        return Regex.Replace(
            Regex.Replace(
                Regex.Replace(input, @"([\p{Lu}]+)([\p{Lu}][\p{Ll}])", "$1_$2"), @"([\p{Ll}\d])([\p{Lu}])", "$1_$2"),
            @"[-\s]", "_").ToLower();
    }
    
    public static GrpcElementSearchInfo ToGrpcElementSearchHelper(this string searchPattern, INamespace csharpNamespace)
    {
        return new GrpcElementSearchInfo(searchPattern, csharpNamespace);
    }
}