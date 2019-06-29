using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace WM.UI.Mvc.App_Start
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            RegisterStyleBundles(bundles);
            RegisterScriptBundles(bundles);
            BundleTable.EnableOptimizations = true;
        }

        #region Scripts
        private static void RegisterScriptBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundle/scripts").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/umd/popper.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/knockout-3.4.2.js"
                        ));

            //Grid mvc
            bundles.Add(new ScriptBundle("~/bundle/gridmvc/scripts").Include(
                      "~/Scripts/GridMvc/ladda-bootstrap/*.min.js",
                      "~/Scripts/GridMvc/URI.js",
                      "~/Scripts/GridMvc/gridmvc.min.js",
                      "~/Scripts/GridMvc/gridmvc-ext.js",
                      "~/Scripts/GridMvc/gridmvc.lang.tr.js",
                      "~/Scripts/GridMvc/gridmvc.customwidgets.js"));

            //alert
            bundles.Add(new ScriptBundle("~/bundle/alert/scripts").Include(
                     "~/Scripts/alertify.min.js"));

            //alert - toastr
            bundles.Add(new ScriptBundle("~/bundle/alert_toastr/scripts").Include(
                     "~/Scripts/toastr.min.js"));

            //bootstrap-select
            bundles.Add(new ScriptBundle("~/bundle/bootstrap-select/scripts").Include(
                     "~/Scripts/bootstrap-select.min.js"));

            //bootstrap-datepicker
            bundles.Add(new ScriptBundle("~/bundle/bootstrap-datepicker/scripts").Include(
                     "~/Scripts/bootstrap-datepicker.min.js",
                     "~/Scripts/locales/bootstrap-datepicker.tr.min.js",
                     "~/Scripts/bootstrap-datepicker-ekle.js"
                     ));

            //Pivot
            bundles.Add(new ScriptBundle("~/bundle/queryui/scripts").Include(
                      "~/Scripts/jquery-ui-1.12.1.js",
                      "~/Scripts/jquery.peity.js",
                      "~/Scripts/canvasjs.js",
                      "~/Scripts/pagination.js",
                      "~/Scripts/paging.js"
                      ));

            //Pivot
            bundles.Add(new ScriptBundle("~/bundle/pivot/scripts").Include(                    
                      "~/Scripts/pivot.js",
                      "~/Scripts/tips_data.min.js",
                      "~/Scripts/c3.min.js",
                      "~/Scripts/jquery.ui.touch-punch.min.js",
                      "~/Scripts/d3/d3.min.js",
                      "~/Scripts/pivot.min.js",
                      "~/Scripts/c3_renderers.min.js",
                      "~/Scripts/jquery-ui.min.js"
                      ));

            //vis
            bundles.Add(new ScriptBundle("~/bundle/vis/scripts").Include(
                      "~/vis/vis.min.js",
                      "~/vis/vis-network.min.js"
                      ));

            //ajax
            bundles.Add(new ScriptBundle("~/bundle/ajax").Include(
                      "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            // date-time picker
            bundles.Add(new ScriptBundle("~/bundle/datetimepicker/scripts").Include(
                      "~/Scripts/datetimepicker/moment.js",
                      "~/Scripts/datetimepicker/bootstrap-datetimepicker.min.js")
                      );

            //validation
            bundles.Add(new ScriptBundle("~/bundle/validation/scripts").Include(
                      "~/Scripts/jquery.validate*"));

            //Scripts Kullanılabilir
            bundles.Add(new ScriptBundle("~/bundle/scripts2").Include(
                      "~/Scripts/MyJavaScript.js"));


            //Scripts Kullanılabilir
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundle/modernizr").Include(
                        "~/Scripts/modernizr-*",
                        "~/Scripts/bootstrap-toggle.js"));
        }
        #endregion

        #region Styles
        private static void RegisterStyleBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/bundle/style")
                .Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/Site.css",
                      "~/Content/paging.css"
                        )
                .Include("~/Content/css/font-awesome.min.css", new CssRewriteUrlTransform()));

            // --jquery-ui
            bundles.Add(new StyleBundle("~/bundle/jqueryui/style").Include(
                      "~/Content/themes/base/jquery-ui.min.css", new CssRewriteUrlTransform()));

            // -- Grid mvc
            bundles.Add(new StyleBundle("~/bundle/gridmvc/style").Include(
                      "~/Content/Gridmvc.css",
                      "~/Content/gridmvc.datepicker.min.css"
                      ));

            // -- Network
            bundles.Add(new StyleBundle("~/bundle/network/style").Include(
                      "~/vis/vis-network.min.css"
                      ));

            // -- alert toastr
            bundles.Add(new StyleBundle("~/bundle/alert_toastr/style").Include(
                      "~/Content/toastr.min.css"
                      ));

            // -- alert
            bundles.Add(new StyleBundle("~/bundle/alert/style").Include(
                      //"~/Content/alertifyjs/alertify.min.css",
                      "~/Content/alertifyjs/alertify.min.css",
                      "~/Content/alertifyjs/themes/default.min.css",
                      "~/Content/alertifyjs/themes/semantic.min.css",
                      "~/Content/alertifyjs/themes/bootstrap.min.css"
                      ));

            // -- bootstrap-datepicker
            bundles.Add(new StyleBundle("~/bundle/bootstrap-datepicker/style").Include(
                      "~/Content/bootstrap-datepicker.min.css"
                      ));

            // -- bootstrap-select
            bundles.Add(new StyleBundle("~/bundle/bootstrap-select/style").Include(
                      "~/Content/bootstrap-select.min.css"));

            // -- pivot table 
            bundles.Add(new StyleBundle("~/bundle/pivot/style").Include(
                      "~/Content/pivot.css",
                      "~/Content/c3.min.css"
                      ));

            // -- date-time picker
            bundles.Add(new StyleBundle("~/bundle/datetimepicker/style").Include(
                      "~/Content/datetimepicker/bootstrap-datetimepicker.min.css"));
        }
        #endregion
    }
}
