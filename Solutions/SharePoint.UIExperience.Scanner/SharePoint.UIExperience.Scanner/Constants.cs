using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.UIExperience.Scanner
{
    public static class Constants
    {
        // Flight based scanner exclusions
        public const string ManagedMetadataNavigationSupport = "RenderMetadataNavInFiltersPane";
        public static readonly string[] ExcludedFromScan = { ManagedMetadataNavigationSupport };

        public static bool IsExcludedFromScan(string flight)
        {
            return ExcludedFromScan.Contains(flight);
        }
    }
}
