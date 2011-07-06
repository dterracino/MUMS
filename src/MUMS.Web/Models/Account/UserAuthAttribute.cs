using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace MUMS.Web.Models
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UserAuthAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool development;
            if (bool.TryParse(ConfigurationManager.AppSettings["IsDevelopment"], out development) && development)
                return true;

            if (CurrentSession.User == null)
                return false;

            return base.AuthorizeCore(httpContext);
        }
    }
}