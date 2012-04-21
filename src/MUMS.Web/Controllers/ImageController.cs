using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;

namespace MUMS.Web.Controllers
{
    public partial class ImageController : Controller
    {
        public virtual ActionResult Index()
        {
            return View();
        }

        public virtual ActionResult TvShow(string title, int season)
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
                int resSeason;
                string path = GetImagePath(title, season, out resSeason);
                if (System.IO.File.Exists(path) && resSeason == season)
                {
                    System.IO.File.Copy(path, serverPath);
                }
                else
                {
                    var client = new WebClient();
                    string url = "http://placehold.it/400x200/ffffff/000000&text=" + HttpUtility.UrlEncode(string.Format("{0} S{1:00}", title, season));
                    client.DownloadFile(url, serverPath);
                }
            }

            return File(serverPath, contentType);
        }

        private string GetImagePath(string title, int season, out int resultingSeason)
        {
            string imgSearchPath;

            if (season <= -1)
                imgSearchPath = @"seasonE:\Serier\" + title + @"\";
            else if (season == 0)
                imgSearchPath = @"seasonE:\Serier\" + title + @"\* All seasons";
            else
                imgSearchPath = @"seasonE:\Serier\" + title + @"\Season " + season;

            string crc = XBMCUtils.CRC(imgSearchPath);
            string path = string.Format(@"C:\Users\TV\AppData\Roaming\XBMC\userdata\Thumbnails\Video\{0}\{1}.tbn", crc.First(), crc);

            if (!System.IO.File.Exists(path))
            {
                if (season > 0)
                    return GetImagePath(title, 0, out resultingSeason);
                else if (season == 0)
                    return GetImagePath(title, -1, out resultingSeason);
            }

            resultingSeason = season;
            return path;
        }
    }
}
