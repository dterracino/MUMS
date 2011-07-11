using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace MUMS.Web.Config
{
    public static class CookieTriggers
    {
        /// <summary>
        /// Returns an ASPNET1Configuration instance
        /// </summary>
        public static List<CookieTrigger> GetConfig()
        {
            return ConfigurationManager.GetSection("cookieConfig") as List<CookieTrigger>;
        }
    }
}