using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MUMS.Web.Models;
using System.Web.Security;
using System.Net;
using System.IO;
using MUMS.Data;

namespace MUMS.Web.Controllers
{
    public partial class AccountController : MumsController
    {
        public virtual ActionResult LogOn(string returnUrl)
        {
            if (LoginIfCookieExists())
            {
                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction(MVC.Root.Index());
            }

            SetViewBagRpxFrameUrl();
            return View();
        }

        private bool LoginIfCookieExists()
        {
            // If the user is already logged in, we don't want to start redirecting.
            // An already logged in user that arrives at the login page is probably sent here because of
            // lacking roles. If we start redirecting to the ReturnUrl in this case, we will be looping.
            if (CurrentSession.User != null)
                return false;

            string identity = HttpContext.User.Identity.Name;
            if (string.IsNullOrWhiteSpace(identity))
                return false;

            var user = MumsMembershipProvider.GetUser(identity);
            if (user == null)
                return false;

            CurrentSession.User = user;
            return true;
        }

        public virtual ActionResult LogOff()
        {
            Response.Cookies.Remove(FormsAuthentication.FormsCookieName);
            FormsAuthentication.SignOut();
            Session.Abandon();

            return RedirectToAction(MVC.Account.LogOn());
        }

        private void SetViewBagRpxFrameUrl()
        {
            var returnUrl = new Uri(string.Format(
                "{0}://{1}/{2}/{3}",
                Request.Url.Scheme,
                Request.Url.Authority,
                MVC.Account.Name,
                MVC.Account.ActionNames.Rpx
            )).ToString();

            ViewBag.RpxFrameUrl = string.Format(
                "https://chsk.rpxnow.com/openid/embed?language_preference=sv-SE&token_url={0}",
                Url.Encode(returnUrl)
            );
        }

        public virtual ActionResult Rpx()
        {
            string apiKey = "0e4c2903356fe62c797b6cd5aa297755fca137ab";
            string token = Request.Form["token"];
            string url = string.Format("https://rpxnow.com/api/v2/auth_info?apiKey={0}&token={1}", apiKey, token);

            var request = WebRequest.Create(url);
            var response = request.GetResponse();

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                RpxResponse rpxResponse = null;
                rpxResponse = RpxResponse.Deserialize(reader.ReadToEnd());

                if (rpxResponse == null)
                    throw new ApplicationException("RPX Response was null.");

                if (rpxResponse.Status != "ok")
                    throw new ApplicationException("Authorization failed, RPX returned status: " + rpxResponse.Status);

                if (rpxResponse.Profile == null || string.IsNullOrWhiteSpace(rpxResponse.Profile.Identifier))
                    throw new ApplicationException("Did not recieve any profile data from RPX call.");

                return HandleAuthenticatedUser(rpxResponse.Profile.Identifier);
            }
        }

        private ActionResult HandleAuthenticatedUser(string authenticatedToken)
        {
            using (var ctx = new MumsDataContext())
            {
                var user = ctx.User.SingleOrDefault(u => u.Token == authenticatedToken);
                if (user == null)
                {
                    TempData.Add("NotFound", authenticatedToken);
                    return RedirectToAction(MVC.Account.LogOn());
                }

                CurrentSession.User = user;
            }

            var expiration = DateTime.Now.Date.AddDays(365);
            int timeoutMinutes = (int)(expiration - DateTime.Now).TotalMinutes;
            var ticket = new FormsAuthenticationTicket(authenticatedToken, true, timeoutMinutes);

            string ticketString = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, ticketString)
            {
                Expires = expiration
            };

            FormsAuthentication.SetAuthCookie(authenticatedToken, true);

            HttpContext.Response.Cookies.Add(cookie);

            return RedirectToAction(MVC.Root.Index());
        }
    }
}
