using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.VSTools
{
    public static class Constants
    {
        public const int PROJECT_ITEM_DELAY_IN_MS = 1000; //delay to process project items (in milliseconds)
        public const string OUTPUT_WINDOW_PANE_NAME = "PnP Deployment Tools";
        public const string PROJECT_NAME = "Provisioning.VSTools";
        public const string DEFAULT_TARGET_SITE = "https://yourtenant.sharepoint.com/sites/testsite";
        public static string[] ExtensionsToIgnore = new string[] { 
            ".map", ".bundle"  //do not handle .bundle or .map files the other files that are part of the bundle should be handled.
        };
    }
}
