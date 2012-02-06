using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SignalR.Client.Hubs;
using SignalR.Client;
using SignalR.Hubs;
using MUMS.Web.Models.Splashify;
using System.Threading;
using System.Diagnostics;

namespace MUMS.Web.Controllers
{
    public partial class SplashifyController : Controller
    {
        private static HubConnection m_hubConnection;
        private static IHubProxy m_proxy;

        private IHubProxy GetProxy()
        {
            if (m_hubConnection == null)
            {
                string url = "http://" + Request.Url.Authority + "/signalr/hubs";
                m_hubConnection = new HubConnection(url);
                m_hubConnection.Start().Wait();
            }

            if (m_proxy == null)
                m_proxy = m_hubConnection.CreateProxy("MUMS.Web.SplashifyHub");

            if (!m_hubConnection.IsActive)
                m_hubConnection.Start().Wait();

            return m_proxy;
        }

        public virtual ActionResult Index()
        {
            return View();
        }

        public virtual ActionResult Notify(PlayingModel model)
        {
            GetProxy().Invoke<SplashifyHub>("SetPlaying", model).Wait();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
