using System.Web;
using System.Web.Optimization;

namespace Paladar_Emelina
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new StyleBundle("~/Content/css_global").Include(
                "~/Content/bootstrap.min.css",
                "~/Content/ionicons.min.css",
                "~/Content/flexslider.css",
                "~/Content/jasny-bootstrap.min.css",
                "~/Content/style.css"
                ));

            bundles.Add(new StyleBundle("~/Content/css_presentacion").Include(
                "~/Content/settings.css",
                "~/Content/layers.css",
                "~/Content/navigation.css"
                ));

            bundles.Add(new StyleBundle("~/Content/css_combo_metronic").Include(
                "~/Content/bootstrap-select/bootstrap-select.min.css",
                "~/Content/select2/select2.css"));

            bundles.Add(new ScriptBundle("~/Scripts/js_global").Include(
                "~/Scripts/jquery.min.js",
                "~/Scripts/jquery.easing.min.js",
                "~/Scripts/bootstrap.min.js",
                "~/Scripts/jquery.flexslider-min.js",
                "~/Scripts/jquery.stellar.min.js",
                "~/Scripts/jquery.sticky.js",
                "~/Scripts/jquery.countdown.min.js",
                "~/Scripts/jasny-bootstrap.min.js",
                "~/Scripts/bootstrap-hover-dropdown.min.js",
                "~/Scripts/template-custom.js",
                "~/Scripts/scripts.js"
                ));

            bundles.Add(new ScriptBundle("~/Scripts/js_slider").Include(
                "~/Scripts/jquery.themepunch.tools.min.js",
                "~/Scripts/jquery.themepunch.revolution.min.js",
                "~/Scripts/revolution.extension.video.min.js",
                "~/Scripts/revolution.extension.slideanims.min.js",
                "~/Scripts/revolution.extension.layeranimation.min.js",
                "~/Scripts/revolution.extension.navigation.min.js",
                "~/Scripts/revolution.extension.parallax.min.js"
                ));

            bundles.Add(new ScriptBundle("~/Scripts/js_isotope").Include(
                "~/Scripts/isotope.js",
                "~/Scripts/imagesloaded.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/js_combo_metronic").Include(
                "~/Content/bootstrap-select/bootstrap-select.min.js",
                "~/Content/select2/select2.min.js",
                "~/Scripts/components-dropdowns.js"));

            bundles.Add(new StyleBundle("~/assets/css_hostal").Include(
                "~/assets/bootstrap-material-design-font/css/material.css",
                "~/Content/ionicons.min.css",
                "~/assets/fonts/style.css",
                "~/assets/tether/tether.min.css",
                "~/assets/bootstrap/css/bootstrap.min.css",
                "~/assets/animate.css/animate.min.css",
                "~/assets/dropdown/css/style.css",
                "~/assets/theme/css/style.css",
                "~/assets/mobirise/css/mbr-additional.css"));

            bundles.Add(new ScriptBundle("~/bundles/js_hostal").Include(
                "~/assets/web/assets/jquery/jquery.min.js",
                "~/assets/tether/tether.min.js",
                "~/assets/bootstrap/js/bootstrap.min.js",
                "~/assets/smooth-scroll/smooth-scroll.js",
                "~/assets/bootstrap-carousel-swipe/bootstrap-carousel-swipe.js",
                "~/assets/jquery-mb-ytplayer/jquery.mb.ytplayer.min.js",
                "~/assets/viewport-checker/jquery.viewportchecker.js",
                "~/assets/jarallax/jarallax.js",
                "~/assets/dropdown/js/script.min.js",
                "~/assets/touch-swipe/jquery.touch-swipe.min.js",
                "~/assets/theme/js/script.js",
                "~/Scripts/scripts.js"));
        }
    }
}
