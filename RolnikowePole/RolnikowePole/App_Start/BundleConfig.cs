using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace RolnikowePole.App_Start
{
    public class BundleConfig
    {
        /// <summary>
        /// Bundle == Usuniecie przestrzeni i komentarzy. Dodatkowo zawrzenie kilka styli lub skryptów w jeden bundle
        /// </summary>
        /// <param name="bundles"></param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jqueryAndjqueryUI").Include(
                                         "~/Scripts/jquery-{version}.js",
                                         "~/Scripts/jquery-ui-{version}.js"));
                                         

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                                        "~/Content/themes/base/core.css",
                                        "~/Content/themes/base/autocomplete.css",
                                        "~/Content/themes/base/theme.css",
                                        "~/Content/themes/base/menu.css"));

            //Bundle for homepage
            bundles.Add(new StyleBundle("~/Content/BootstrapAndCssForHomepage").Include(
                                        "~/Content/bootstrap.min.css",
                                        "~/Content/css/shop-homepage.css"
                                        ));


            //Bundle for item
            bundles.Add(new StyleBundle("~/Content/Bootstrap&CssForItem").Include(
                                        "~/Content/bootstrap.min.css",
                                        "~/Content/css/shop-item.css"
                                        ));

            //Bundle Scripts for homepage & item
            bundles.Add(new ScriptBundle("~/bundles/jqueryAndBootstrapBundle").Include(
                                         "~/Scripts/jquery-{version}.js",
                                         "~/Scripts/bootstrap.bundle.min.js"
                                         ));
        }   
    }
}