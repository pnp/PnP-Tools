// ------------------------------------------------------------------------------
//The MIT License(MIT)

//Copyright(c) 2015 Office Developer
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
// ------------------------------------------------------------------------------

using System.Web;
using System.Web.Optimization;
namespace TIP.Dashboard
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/javascript").Include(
                   "~/Scripts/tip.dashboard.js"));

            bundles.Add(new ScriptBundle("~/bundles/metisMenu").Include(
                 "~/Scripts/metisMenu.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/toastr").Include(
                    "~/Scripts/plugins/toastr/toastr.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                    "~/Scripts/angular.js",
                    "~/Scripts/angular-animate.min.js",
                    "~/Scripts/angular-messages.js",
                    "~/Scripts/angular-sanitize.min.js",
                    "~/Scripts/angular-ui/ui-bootstrap.min.js",
                    "~/Scripts/angular-ui/ui-bootstrap-tpls.min.js",
                    "~/Scripts/angular-translate.min.js",
                    "~/Scripts/angular-translate-loader-static-files.min.js",
                    "~/Scripts/angular-translate-loader-url.min.js"));
            
            //Angular Common Modules
            bundles.Add(new ScriptBundle("~/bundles/appCommon").Include(
                    "~/Scripts/app/common/logger/logger.module.js",
                    "~/Scripts/app/common/logger/logger.js",
                    "~/Scripts/app/common/exception/exception.module.js",
                    "~/Scripts/app/common/exception/exception-handler.provider.js",
                    "~/Scripts/app/common/exception/exception.js",
                    "~/Scripts/app/common/spinners/spin.min.js",
                    "~/Scripts/app/common/spinners/angular-spinner.js",
                    "~/Scripts/app/common/excelExport/FileSaver.js",
                    "~/Scripts/app/common/excelExport/json-export-excel.js",
                    "~/Scripts/plugins/chart/Chart.min.js",
                    "~/Scripts/app/common/chart/angular-chart.min.js"));

            //Angular add module
            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                    "~/Scripts/app/app.module.js"));

            //app.dashboard
            bundles.Add(new ScriptBundle("~/bundles/appDashboard").Include(
                   "~/Scripts/app/dashboard/dashboard.module.js",
                   "~/Scripts/app/dashboard/dashboard.controller.js"));

            //app.reports
            bundles.Add(new ScriptBundle("~/bundles/appPrincipals").Include(
                   "~/Scripts/app/principals/principal.module.js",
                   "~/Scripts/app/principals/principal.controller.js",
                   "~/Scripts/app/reports/reports.module.js",
                   "~/Scripts/app/reports/reports.allprincipals.controller.js",
                   "~/Scripts/app/reports/reports.allexpired.controller.js",
                   "~/Scripts/app/reports/reports.expired30.controller.js",
                   "~/Scripts/app/reports/reports.expired60.controller.js",
                   "~/Scripts/app/reports/reports.expired90.controller.js",
                   "~/Scripts/app/reports/reports.apps.all.controller.js",
                   "~/Scripts/app/reports/reports.apps.expired.controller.js"));

            //Angular app.core
            bundles.Add(new ScriptBundle("~/bundles/appCore").Include(
                    "~/Scripts/app/core/core.module.js",
                    "~/Scripts/app/directives/directives.dirPagination.js",
                    "~/Scripts/app/core/config.js",
                    "~/Scripts/app/core/constants.js",
                    "~/Scripts/app/core/application.services.js",
                    "~/Scripts/app/core/principal.services.js",
                    "~/Scripts/app/core/tenant.service.js"));

            //CSS
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/metisMenu.min.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/toastr.min.css",
                      "~/Content/site.css",
                      "~/Content/prism.css",
                      "~/Content/angular-chart.css"));
        }
    }
}
