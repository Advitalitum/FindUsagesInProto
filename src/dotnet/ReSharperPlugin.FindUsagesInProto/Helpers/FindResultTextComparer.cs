using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Search;

namespace ReSharperPlugin.FindUsagesInProto;

public class FindResultTextComparer : IComparer<FindResultText>
{
    private const string VendorProtogenPart = "vendor.protogen";

    private FindResultTextComparer()
    {
    }

    public static readonly FindResultTextComparer Instance = new();

    public int Compare(FindResultText left, FindResultText right)
    {
        if (left is null || right is null)
        {
            throw new ArgumentNullException();
        }

        var leftFullPath = left.SourceFile.GetLocation().FullPath;
        var rightFullPath = right.SourceFile.GetLocation().FullPath;

        var leftContainsVendorProtogen = leftFullPath.Contains(VendorProtogenPart);
        var rightContainsVendorProtogen = rightFullPath.Contains(VendorProtogenPart);

        if (rightContainsVendorProtogen && leftContainsVendorProtogen is false)
        {
            return -1;
        }

        if (leftContainsVendorProtogen && rightContainsVendorProtogen is false)
        {
            return 1;
        }

        return 0;
    }
}