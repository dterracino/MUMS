using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MUMS.Utorrent;
using System.Configuration;
using MUMS.Data;

namespace MUMS.Web.Models
{
    public class CurrentSession
    {
        private static string m_Key_UtorrentClient = "UtorrentClient";

        public static UtorrentClient Client
        {
            get 
            {
                var client = HttpContext.Current.Session[m_Key_UtorrentClient] as UtorrentClient;
                if (client == null)
                {
                    client = CreateClient();
                    HttpContext.Current.Session[m_Key_UtorrentClient] = client;
                }

                return client;
            }
        }

        public static User User { get; set; }

        private static UtorrentClient CreateClient()
        {
            string uTorrentUserName = ConfigurationManager.AppSettings["uTorrentUserName"];
            string uTorrentPassword = ConfigurationManager.AppSettings["uTorrentPassword"];
            string uTorrentAddress = ConfigurationManager.AppSettings["uTorrentAddress"];
            return new UtorrentClient(uTorrentUserName, uTorrentPassword, uTorrentAddress);
        }
    }
}