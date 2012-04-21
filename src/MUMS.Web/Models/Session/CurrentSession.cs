using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using MUMS.Data;
using UTorrentAPI;

namespace MUMS.Web.Models
{
    public class CurrentSession
    {
        private static string m_Key_UtorrentClient = "UtorrentClient";

        public static UTorrentClient Client
        {
            get 
            {
                var client = HttpContext.Current.Session[m_Key_UtorrentClient] as UTorrentClient;
                if (client == null)
                {
                    client = CreateClient();
                    HttpContext.Current.Session[m_Key_UtorrentClient] = client;
                }

                return client;
            }
        }

        public static User User { get; set; }

        private static UTorrentClient CreateClient()
        {
            string uTorrentUserName = ConfigurationManager.AppSettings["uTorrentUserName"];
            string uTorrentPassword = ConfigurationManager.AppSettings["uTorrentPassword"];
            string uTorrentAddress = ConfigurationManager.AppSettings["uTorrentAddress"];
            Uri uTorrentUri;

            if (Uri.TryCreate(uTorrentAddress, UriKind.RelativeOrAbsolute, out uTorrentUri))
                return new UTorrentClient(uTorrentUri, uTorrentUserName, uTorrentPassword);
            else
                throw new ApplicationException("uTorrentAddress \"" + uTorrentAddress + "\" in web.config is malformed or missing");
        }
    }
}