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
        }
    }
}