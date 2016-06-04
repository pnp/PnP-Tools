using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.VSTools
{
    public static class Constants
    {
        public const string ProjectName = "Provisioning.VSTools";
        public static string[] ExtensionsToIgnore = new string[] { 
            ".map", ".bundle"  //do not handle .bundle or .map files the other files that are part of the bundle should be handled.
        };
    }
}
