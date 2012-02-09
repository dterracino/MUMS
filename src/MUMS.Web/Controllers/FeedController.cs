using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ServiceModel.Syndication;
using MUMS.Data;
using System.Collections.ObjectModel;
using System.Xml;
using System.Text;
using System.Xml.Linq;
using System.Threading;
using System.Globalization;

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

        public static List<RssEpisodeItems> GetItems(int limit = 100)
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

        public void GenerateRss(List<RssEpisodeItems> items)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("sv-SE");

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

                        from item in items
                        select new XElement("item",
                            new XElement("title", item.ReleaseName),
                            new XElement("description", GetContent(item)),
                            new XElement("id", item.EnclosureUrl),
                            new XElement("pubDate", item.PubDate.ToString("r")),
                            new XElement("link", item.EnclosureUrl),
                            new XElement("enclosure",
                                new XAttribute("url", item.EnclosureUrl),
                                new XAttribute("length", item.EnclosureLength),
                                new XAttribute("type", "application/x-bittorrent")
                            )
                        )
                    )
                )
            );

            using (var writer = new XmlTextWriter(Response.OutputStream, Encoding.UTF8))
                document.WriteTo(writer);
        }

        private object GetContent(RssEpisodeItems item)
        {
            string imgUrl = "http://mums.chsk.se/image/tvshow/?title=" + item.ShowName;
            string html = string.Format(
                "<img src=\"{0}\" alt=\"{1}\" /><p>Tillagd {2:dddd\\e\\n \\d\\e\\n d MMMM, HH:mm}</p><p><a href=\"{3}\">Source url</a></p>",
                imgUrl,
                item.ShowName,
                item.Added,
                item.SourceUrl
            );

            return new XCData(html);
        }
    }
}
