using System.Web;
using System.Web.Optimization;

namespace OnlineStore
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {                   
            bundles.Add(new ScriptBundle("~/bundles/cart").Include(
                      "~/Scripts/cart.js"));

            bundles.Add(new ScriptBundle("~/bundles/checkout").Include(
                      "~/Scripts/checkout.js"));

            bundles.Add(new ScriptBundle("~/bundles/comments").Include(
                      "~/Scripts/comments.js"));

            bundles.Add(new ScriptBundle("~/bundles/feedback").Include(
                      "~/Scripts/feedback.js"));

            bundles.Add(new ScriptBundle("~/bundles/newsletter").Include(
                      "~/Scripts/newsletter.js"));

            bundles.Add(new StyleBundle("~/Content/custom").Include(
                      "~/Content/styles/custom.css"));

            bundles.Add(new ScriptBundle("~/bundles/admin-login").Include(
                      "~/Scripts/admin-login.js"));

            bundles.Add(new ScriptBundle("~/bundles/admin-scripts").Include(
                      "~/Scripts/admin-scripts.js"));

            bundles.Add(new StyleBundle("~/Content/admin-custom").Include(
                      "~/Content/styles/admin-custom.css"));
        }
    }
}
