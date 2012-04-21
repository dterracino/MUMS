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
            return Rss();
        }

        public virtual ActionResult Rss()
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
                            new XElement("title", new XCData(string.Format("{0}, S{1:00}E{2:00}", item.ShowName, item.Season, item.Episode))),
                            new XElement("description", GetContent(item)),
                            new XElement("id", item.EnclosureUrl),
                            new XElement("pubDate", item.PubDate.ToString("r")),
                            new XElement("link", new XCData(item.SourceUrl ?? item.EnclosureUrl)),
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
            {
                writer.Indentation = 4;
                writer.Formatting = Formatting.Indented;
                document.WriteTo(writer);
            }
        }

        private XCData GetContent(RssEpisodeItems item)
        {
            string imgUrl = string.Format(
                "http://mums.chsk.se/image/tvshow/?title={0}&season={1}",
                item.ShowName,
                item.Season
            );

            string html = string.Format(
                "<a href=\"{0}\"><img src=\"{0}\" alt=\"{1}\" /></a><p>Tillagd {2:dddd\\e\\n \\d\\e\\n d MMMM, HH:mm}</p>",
                imgUrl,
                item.ShowName + " (id:" + item.RssEpisodeItemId + ")",
                item.Added
            );

            return new XCData(html);
        }
    }
}
