using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

namespace MUMS.Web.Config
{
    /// <summary>
    /// A C# model of a cookieTrigger from web.config
    /// </summary>
    public class CookieTrigger
    {
        /// <summary>
        /// The list of domains that trigger this cookie.
        /// </summary>
        public List<string> TriggerDomains { get; set; }
        
        /// <summary>
        /// The list of cookie-values associated with this trigger.
        /// </summary>
        public List<Cookie> Cookies { get; set; }
    }
}