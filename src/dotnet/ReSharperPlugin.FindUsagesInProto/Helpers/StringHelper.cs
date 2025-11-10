using System.Text.RegularExpressions;

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
}