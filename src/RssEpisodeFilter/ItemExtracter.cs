namespace MUMS.RssEpisodeFilter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Xml.Linq;
    using MUMS.RssEpisodeFilter.Properties;

    public class ItemExtracter
    {
        public static List<Episode> GetItems()
        {
            var items = new List<Episode>();

            try
            {
                var client = new WebClient();
                string feedContent = client.DownloadString(Settings.Default.FeedUrl);
                
                using (var reader = new StringReader(feedContent))
                {
                    XElement warez = XElement.Load(reader);
                    Episode episode;
                    foreach (var item in warez.Descendants("item"))
                    {
                        if (TryParseItem(item, out episode))
                            items.Add(episode);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.PrintStatus(ConsoleColor.Red, "Exception", "Exception in GetItems:\n" + ex.ToString());
            }

            return items;
        }

        private static bool TryParseItem(XElement item, out Episode episode)
        {
            episode = null;
            Uri torrentUrl;

            XElement enclosure = item.Element("enclosure");
            string link = enclosure.Attribute("url").Value;

            if (!Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out torrentUrl))
            {
                Logging.PrintInvalid("Invalid torrent url\t" + link);
                return false;
            }

            long length;
            if (!long.TryParse(enclosure.Attribute("length").Value, out length))
            {
                Logging.PrintInvalid("Invalid torrent size\t" + enclosure.Attribute("length"));
                return false;
            }

            Uri sourceUrl;
            Uri.TryCreate(item.Element("link").Value, UriKind.RelativeOrAbsolute, out sourceUrl);

            episode = new Episode
            {
                Title = (item.Element("title").Value ?? string.Empty).Trim(),
                PubDate = DateTime.Parse(item.Element("pubDate").Value),
                TorrentUrl = torrentUrl,
                TorrentSize = length,
                SourceUrl = sourceUrl
            };

            return true;
        }
    }

}
