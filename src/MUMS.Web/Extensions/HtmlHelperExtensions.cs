using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MUMS.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString CompiledStylesheet(this HtmlHelper helper)
        {
            #if DEBUG
            return MvcHtmlString.Empty;
            #else
            string stylesheetPath = Links.Content.compiled_css;
            return helper.Action(MVC.Content.Stylesheet(stylesheetPath));
            #endif
        }

        public static MvcHtmlString CompiledScript(this HtmlHelper helper)
        {
            #if DEBUG
            return MvcHtmlString.Empty;
            #else
            string scriptPath = Links.Scripts.compiled_js;
            return helper.Action(MVC.Content.Script(scriptPath));
            #endif
        }

        public static MvcHtmlString Stylesheet(this HtmlHelper helper, string stylesheetPath)
        {
            #if DEBUG
            return helper.Action(MVC.Content.Stylesheet(stylesheetPath));
            #else
            return MvcHtmlString.Empty;
            #endif
        }

        public static MvcHtmlString Script(this HtmlHelper helper, string scriptPath)
        {

            #if DEBUG
            return helper.Action(MVC.Content.Script(scriptPath));
            #else
            return MvcHtmlString.Empty;
            #endif
            
        }

        public static MvcHtmlString ScriptCDN(this HtmlHelper helper, string url)
        {
            return MvcHtmlString.Create("<script type=\"text/javascript\" src=\"" + url + "\"></script>");
        }
    }
}