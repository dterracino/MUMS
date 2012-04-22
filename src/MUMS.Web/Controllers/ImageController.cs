using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;
using TvdbLib;
using TvdbLib.Data;
using MUMS.Data;
using System.Configuration;

namespace MUMS.Web.Controllers
{
    public partial class ImageController : Controller
    {
        public virtual ActionResult Index()
        {
            return View();
        }

        public virtual ActionResult Episode(int id)
        {
            using (var ctx = new MumsDataContext())
            {
                var data = ctx.RssEpisodeItems.Where(e => e.RssEpisodeItemId == id).Select(e => new
                {
                    e.ShowName,
                    e.Season,
                    e.Episode
                }).SingleOrDefault();

                if (data == null)
                    return new EmptyResult();

                return TvShow(data.ShowName, data.Season, data.Episode);
            }
        }

        public virtual ActionResult TvShow(string title, int season, int episode)
        {
            string fileName = string.Format("{0} {1}.jpg", title, season).Replace(' ', '_');
            string contentType = "image/jpeg";
            string serverPath = Server.MapPath("~/Content/tvshow/" + fileName);

            if (string.IsNullOrWhiteSpace(title))
            {
                serverPath = Server.MapPath("~/Content/images/mums.png");
                contentType = "image/png";
            }

            if (!System.IO.File.Exists(serverPath))
            {
                if (!DownloadImage(serverPath, title, season, episode))
                {
                    var client = new WebClient();
                    string url = "http://placehold.it/400x200/ffffff/000000&text=" + HttpUtility.UrlEncode(string.Format("{0} S{1:00}", title, season));
                    client.DownloadFile(url, serverPath);
                }
            }

            return File(serverPath, contentType);
        }

        private bool DownloadImage(string serverPath, string title, int season, int episode)
        {
            string apiKey = ConfigurationManager.AppSettings["tvdb.API.key"];
            if (string.IsNullOrWhiteSpace(apiKey))
                return false;

            TvdbHandler handler = new TvdbHandler(apiKey);

            var searchResult = handler.SearchSeries(title);
            if (searchResult != null && searchResult.Count > 0)
            {
                var result = searchResult.First();
                if (result.Banner.LoadBanner())
                {
                    result.Banner.BannerImage.Save(serverPath);
                    return true;
                }
            }

            return false;
        }
    }
}
