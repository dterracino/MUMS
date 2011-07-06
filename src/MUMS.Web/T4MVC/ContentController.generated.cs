// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments
#pragma warning disable 1591
#region T4MVC

using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using T4MVC;
namespace MUMS.Web.Controllers {
    public partial class ContentController {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ContentController() { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected ContentController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result) {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult Stylesheet() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.Stylesheet);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult Script() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.Script);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult JsonContract() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.JsonContract);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ContentController Actions { get { return MVC.Content; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Content";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass {
            public readonly string Stylesheet = "Stylesheet";
            public readonly string Script = "Script";
            public readonly string JsonContract = "JsonContract";
        }


        static readonly ViewNames s_views = new ViewNames();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewNames Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewNames {
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class T4MVC_ContentController: MUMS.Web.Controllers.ContentController {
        public T4MVC_ContentController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult Stylesheet(string stylesheetPath) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.Stylesheet);
            callInfo.RouteValueDictionary.Add("stylesheetPath", stylesheetPath);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Script(string scriptPath) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.Script);
            callInfo.RouteValueDictionary.Add("scriptPath", scriptPath);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult JsonContract(object obj) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.JsonContract);
            callInfo.RouteValueDictionary.Add("obj", obj);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
