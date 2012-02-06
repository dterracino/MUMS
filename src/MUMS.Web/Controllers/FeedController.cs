using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ServiceModel.Syndication;
using MUMS.Web.Models.Data;
using System.Collections.ObjectModel;
using System.Xml;
using System.Text;
using System.Xml.Linq;

namespace MUMS.Web.Controllers
{
    public partial class FeedController : Controller
    {
        public virtual ActionResult Index()
        {
            GenerateRss(GetItems());
            return new EmptyResult();
        }

        public virtual ActionResult Html()
        {
            return View(GetItems());
        }

        public static List<RssEpisodeItem> GetItems(int limit=100)
        {
            using (var ctx = new MumsDataContext())
            {
                return ctx.RssEpisodeItems
                    .Where(e => e.Download)
                    .OrderByDescending(e => e.Added)
                    .Take(limit)
                    .ToList();
            }
        }

        public void GenerateRss(List<RssEpisodeItem> items)
        {
            Response.Clear();
            Response.ContentType = "application/rss+xml";
            Response.ContentEncoding = Encoding.UTF8;

            var document = new XDocument(
                new XElement("rss", new XAttribute("version", "2.0"),
                    new XElement("channel",
                        new XElement("title", "MUMS RssEpisodeFilter"),
                        new XElement("link", Request.Url.ToString()),
                        new XElement("pubDate", DateTime.Now.ToString("r")),
                        new XElement("generator", "MUMS.RssEpisodeFilter"),

                        from v in items
                        select new XElement("item",
                            new XElement("title", v.ReleaseName),
                            new XElement("id", v.EnclosureUrl),
                            new XElement("pubDate", v.PubDate.ToString("r")),
                            new XElement("enclosure",
                                new XAttribute("url", v.EnclosureUrl),
                                new XAttribute("length", v.EnclosureLength),
                                new XAttribute("type", "application/x-bittorrent")
                            )
                        )
                    )
                )
            );

            using (var writer = new XmlTextWriter(Response.OutputStream, Encoding.UTF8))
                document.WriteTo(writer);
        }
    }
}
