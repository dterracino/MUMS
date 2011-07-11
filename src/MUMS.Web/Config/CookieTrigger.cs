using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

namespace MUMS.Web.Config
{
    public class CookieTrigger
    {
        public List<string> TriggerDomains { get; set; }
        public List<Cookie> Cookies { get; set; }
    }
}