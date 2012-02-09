using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace MUMS.Web.Controllers
{
    public class ImageController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TvShow(string title)
        {
            string fileName = title + ".jpg";
            string contentType = "image/jpeg";
            string serverPath = Server.MapPath("~/Content/tvshow/" + fileName);

            if (string.IsNullOrWhiteSpace(title))
            {
                serverPath = Server.MapPath("~/Content/images/mums.png");
                contentType = "image/png";
            }

            if (!System.IO.File.Exists(serverPath))
            {
                string crc = XBMCUtils.CRC(@"E:\Serier\" + title + @"\");
                string path = string.Format(@"C:\Users\TV\AppData\Roaming\XBMC\userdata\Thumbnails\Video\{0}\{1}.tbn", crc.First(), crc);

                if (System.IO.File.Exists(path))
                    System.IO.File.Copy(path, serverPath);
            }

            return File(serverPath, contentType);
        }
    }
}
