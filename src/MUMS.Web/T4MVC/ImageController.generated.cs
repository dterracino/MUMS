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
    public partial class ImageController {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ImageController() { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected ImageController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result) {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(ActionResult result) {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult Episode() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.Episode);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult TvShow() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.TvShow);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ImageController Actions { get { return MVC.Image; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Image";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "Image";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass {
            public readonly string Index = "Index";
            public readonly string Episode = "Episode";
            public readonly string TvShow = "TvShow";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants {
            public const string Index = "Index";
            public const string Episode = "Episode";
            public const string TvShow = "TvShow";
        }


        static readonly ActionParamsClass_Episode s_params_Episode = new ActionParamsClass_Episode();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Episode EpisodeParams { get { return s_params_Episode; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Episode {
            public readonly string id = "id";
        }
        static readonly ActionParamsClass_TvShow s_params_TvShow = new ActionParamsClass_TvShow();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_TvShow TvShowParams { get { return s_params_TvShow; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_TvShow {
            public readonly string title = "title";
            public readonly string season = "season";
            public readonly string episode = "episode";
        }
        static readonly ViewNames s_views = new ViewNames();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewNames Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewNames {
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class T4MVC_ImageController: MUMS.Web.Controllers.ImageController {
        public T4MVC_ImageController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult Index() {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.Index);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Episode(int id) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.Episode);
            callInfo.RouteValueDictionary.Add("id", id);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult TvShow(string title, int season, int episode) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.TvShow);
            callInfo.RouteValueDictionary.Add("title", title);
            callInfo.RouteValueDictionary.Add("season", season);
            callInfo.RouteValueDictionary.Add("episode", episode);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
