using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace MUMS.Web.Config
{
    /// <summary>
    /// Static access to the cookieTriggers section of web.config.
    /// </summary>
    public static class CookieTriggers
    {
        /// <summary>
        /// Reads the cookieConfig section in web.config and returns any valid cookie-triggers.
        /// </summary>
        public static List<CookieTrigger> GetConfig()
        {
            return ConfigurationManager.GetSection("cookieConfig") as List<CookieTrigger>;
        }
    }
}