using System.Web.Optimization;

namespace Ced.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //    "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //    "~/Scripts/jquery.validate*"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
            //    "~/Scripts/jquery-ui.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/dropzone").Include(
                "~/Scripts/dropzone/dropzone.js"));

            bundles.Add(new ScriptBundle("~/bundles/maxlength").Include(
                "~/Scripts/bootstrap-maxlength.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/timeago").Include(
                "~/Scripts/jquery.timeago.js"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //    "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //    "~/Scripts/bootstrap.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //    "~/Content/bootstrap.css",
            //    "~/Content/dropzone/basic.css",
            //    "~/Content/dropzone/dropzone.css",
            //    "~/Content/site.css",
            //    "~/Content/jquery-ui.min.css"));



            // Homer style
            bundles.Add(new StyleBundle("~/bundles/homer/css").Include(
                "~/Content/style.css", new CssRewriteUrlTransform()));

            // Homer script
            bundles.Add(new ScriptBundle("~/bundles/homer/js").Include(
                "~/Vendor/metisMenu/dist/metisMenu.min.js",
                "~/Vendor/iCheck/icheck.min.js",
                "~/Vendor/peity/jquery.peity.min.js",
                "~/Vendor/sparkline/index.js",
                "~/Scripts/homer.js",
                "~/Scripts/charts.js"));

            // Animate.css
            bundles.Add(new StyleBundle("~/bundles/animate/css").Include(
                "~/Vendor/animate.css/animate.min.css"));

            // Pe-icon-7-stroke
            bundles.Add(new StyleBundle("~/bundles/peicon7stroke/css").Include(
                "~/Icons/pe-icon-7-stroke/css/pe-icon-7-stroke.css", new CssRewriteUrlTransform()));

            // Font Awesome icons style
            bundles.Add(new StyleBundle("~/bundles/font-awesome/css").Include(
                "~/Vendor/fontawesome/css/font-awesome.min.css", new CssRewriteUrlTransform()));

            // Bootstrap style
            bundles.Add(new StyleBundle("~/bundles/bootstrap/css").Include(
                "~/Vendor/bootstrap/dist/css/bootstrap.min.css", new CssRewriteUrlTransform()));

            // Bootstrap
            bundles.Add(new ScriptBundle("~/bundles/bootstrap/js").Include(
                "~/Vendor/bootstrap/dist/js/bootstrap.min.js"));

            // jQuery
            bundles.Add(new ScriptBundle("~/bundles/jquery/js").Include(
                "~/Vendor/jquery/dist/jquery.min.js"));

            // jQuery UI
            bundles.Add(new ScriptBundle("~/bundles/jqueryui/js").Include(
                "~/Vendor/jquery-ui/jquery-ui.min.js"));

            // Flot chart
            bundles.Add(new ScriptBundle("~/bundles/flot/js").Include(
                "~/Vendor/flot/jquery.flot.js",
                "~/Vendor/flot/jquery.flot.tooltip.min.js",
                "~/Vendor/flot/jquery.flot.resize.js",
                "~/Vendor/flot/jquery.flot.pie.js",
                "~/Vendor/flot.curvedlines/curvedLines.js",
                "~/Vendor/jquery.flot.spline/index.js"));

            // Star rating
            bundles.Add(new ScriptBundle("~/bundles/starRating/js").Include(
                "~/Vendor/bootstrap-star-rating/js/star-rating.min.js"));

            // Star rating style
            bundles.Add(new StyleBundle("~/bundles/starRating/css").Include(
                "~/Vendor/bootstrap-star-rating/css/star-rating.min.css", new CssRewriteUrlTransform()));

            // Sweetalert
            bundles.Add(new ScriptBundle("~/bundles/sweetalert/js").Include(
                "~/Vendor/sweetalert/lib/sweetalert2.min.js"));

            // Sweetalert style
            bundles.Add(new StyleBundle("~/bundles/sweetalert/css").Include(
                "~/Vendor/sweetalert/lib/sweetalert2.min.css"));

            // Toastr
            bundles.Add(new ScriptBundle("~/bundles/toastr/js").Include(
                "~/Vendor/toastr/build/toastr.min.js"));

            // Toastr style
            bundles.Add(new StyleBundle("~/bundles/toastr/css").Include(
                "~/Vendor/toastr/build/toastr.min.css"));

            // Nestable
            bundles.Add(new ScriptBundle("~/bundles/nestable/js").Include(
                "~/Vendor/nestable/jquery.nestable.js"));

            // BootstrapTour
            bundles.Add(new ScriptBundle("~/bundles/bootstrapTour/js").Include(
                "~/Vendor/bootstrap-tour/build/js/bootstrap-tour.min.js"));

            // BootstrapTour style
            bundles.Add(new StyleBundle("~/bundles/bootstrapTour/css").Include(
                "~/Vendor/bootstrap-tour/build/css/bootstrap-tour.min.css"));

            // Moment
            bundles.Add(new ScriptBundle("~/bundles/moment/js").Include(
                "~/Vendor/moment/moment.js"));

            // Full Calendar
            bundles.Add(new ScriptBundle("~/bundles/fullCalendar/js").Include(
                "~/Vendor/fullcalendar/dist/fullcalendar.min.js"));

            // Full Calendar style
            bundles.Add(new StyleBundle("~/bundles/fullCalendar/css").Include(
                "~/Vendor/fullcalendar/dist/fullcalendar.min.css"));

            // Chart JS
            bundles.Add(new ScriptBundle("~/bundles/chartjs/js").Include(
                "~/Vendor/chartjs/Chart.min.js"));

            // Datatables
            bundles.Add(new ScriptBundle("~/bundles/datatables/js").Include(
                "~/Vendor/datatables/media/js/jquery.dataTables.min.js",
                "~/Vendor/datatables_plugins/sorting/datetime-moment.js"));

            // Datatables bootstrap
            bundles.Add(new ScriptBundle("~/bundles/datatablesBootstrap/js").Include(
                "~/Vendor/datatables_plugins/integration/bootstrap/3/dataTables.bootstrap.min.js"));

            // Datatables style
            bundles.Add(new StyleBundle("~/bundles/datatables/css").Include(
                "~/Vendor/datatables_plugins/integration/bootstrap/3/dataTables.bootstrap.css"));

            // Xeditable
            bundles.Add(new ScriptBundle("~/bundles/xeditable/js").Include(
                "~/Vendor/xeditable/bootstrap3-editable/js/bootstrap-editable.min.js"));

            // Xeditable style
            bundles.Add(new StyleBundle("~/bundles/xeditable/css").Include(
                "~/Vendor/xeditable/bootstrap3-editable/css/bootstrap-editable.css", new CssRewriteUrlTransform()));

            // Select 2
            bundles.Add(new ScriptBundle("~/bundles/select2/js").Include(
                "~/Vendor/select2-3.5.2/select2.min.js"));

            // Select 2 style
            bundles.Add(new StyleBundle("~/bundles/select2/css").Include(
                "~/Vendor/select2-3.5.2/select2.css",
                "~/Vendor/select2-bootstrap/select2-bootstrap.css"));

            // Touchspin
            bundles.Add(new ScriptBundle("~/bundles/touchspin/js").Include(
                "~/Vendor/bootstrap-touchspin/dist/jquery.bootstrap-touchspin.min.js"));

            // Touchspin style
            bundles.Add(new StyleBundle("~/bundles/touchspin/css").Include(
                "~/Vendor/bootstrap-touchspin/dist/jquery.bootstrap-touchspin.min.css"));

            // Datepicker
            bundles.Add(new ScriptBundle("~/bundles/datepicker/js").Include(
                "~/Vendor/bootstrap-datepicker-master/dist/js/bootstrap-datepicker.min.js"));

            // Datepicker style
            bundles.Add(new StyleBundle("~/bundles/datepicker/css").Include(
                "~/Vendor/bootstrap-datepicker-master/dist/css/bootstrap-datepicker3.min.css"));

            // Summernote
            bundles.Add(new ScriptBundle("~/bundles/summernote/js").Include(
                "~/Vendor/summernote/dist/summernote.min.js"));

            // Summernote style
            bundles.Add(new StyleBundle("~/bundles/summernote/css").Include(
                "~/Vendor/summernote/dist/summernote.css",
                "~/Vendor/summernote/dist/summernote-bs3.css"));

            // Bootstrap checkbox style
            bundles.Add(new StyleBundle("~/bundles/bootstrapCheckbox/css").Include(
                "~/Vendor/awesome-bootstrap-checkbox/awesome-bootstrap-checkbox.css"));

            // Blueimp gallery
            bundles.Add(new ScriptBundle("~/bundles/blueimp/js").Include(
                "~/Vendor/blueimp-gallery/js/jquery.blueimp-gallery.min.js"));

            // Blueimp gallery style
            bundles.Add(new StyleBundle("~/bundles/blueimp/css").Include(
                "~/Vendor/blueimp-gallery/css/blueimp-gallery.min.css", new CssRewriteUrlTransform()));

            // Foo Table
            bundles.Add(new ScriptBundle("~/bundles/fooTable/js").Include(
                "~/Vendor/fooTable/dist/footable.all.min.js"));

            // Foo Table style
            bundles.Add(new StyleBundle("~/bundles/fooTable/css").Include(
                "~/Vendor/fooTable/css/footable.core.min.css", new CssRewriteUrlTransform()));

            // jQuery Validation
            bundles.Add(new ScriptBundle("~/bundles/validation/js").Include(
                "~/Vendor/jquery-validation/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js"));

            // CodeMirror
            bundles.Add(new ScriptBundle("~/bundles/codemirror/js").Include(
                "~/Vendor/codemirror/script/codemirror.js",
                "~/Vendor/codemirror/javascript.js"));

            // CodeMirror style
            bundles.Add(new StyleBundle("~/bundles/codemirror/css").Include(
                "~/Vendor/codemirror/style/codemirror.css"));

            // Chartist
            bundles.Add(new ScriptBundle("~/bundles/chartist/js").Include(
                "~/Vendor/chartist/dist/chartist.min.js"));

            // Chartist style
            bundles.Add(new StyleBundle("~/bundles/chartist/css").Include(
                "~/Vendor/chartist/custom/chartist.css"));

            bundles.Add(new StyleBundle("~/bundles/dropzone/css").Include(
                "~/Content/dropzone/basic.css",
                "~/Content/dropzone/dropzone.css"));
        }
    }
}