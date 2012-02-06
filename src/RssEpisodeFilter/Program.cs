using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;
using MUMS.RssEpisodeFilter.Data;
using MUMS.RssEpisodeFilter.Extensions;
using MUMS.RssEpisodeFilter.Properties;
using System.Diagnostics;
using System.Xml;

namespace MUMS.RssEpisodeFilter
{
    class Program
    {
        static Regex regex1 = new Regex(@"S([0-9]{2})E([0-9]{2})", RegexOptions.IgnoreCase);
        static Regex regex2 = new Regex(@"\.([0-9])([0-9]){2}\.");
        static DateTime BatchDate = DateTime.Now;

        static int Downloads = 0;
        static int Duplicates = 0;

        static void Main(string[] args)
        {
            var items = GetItems();
            items.Shuffle();

            var currentMaxDate = DateTime.MinValue;
            Match match;

            int skipped = 0;
            int processed = 0;

            bool skipDate = false;
            if (args != null && args.Contains("/skip"))
                skipDate = true;

            using (var ctx = new MumsContext())
            {
                if (ctx.RssEpisodeItems.Any())
                {
                    currentMaxDate = ctx.RssEpisodeItems.Max(e => e.PubDate).Date;
                }

                foreach (Episode item in items)
                {
                    if (skipDate && item.PubDate < currentMaxDate)
                    {
                        skipped++;
                        Logging.PrintDateSkipped(item.Title);
                        continue;
                    }

                    match = regex1.Match(item.Title);
                    if (match.Success)
                    {
                        ProcessEpisode(ctx, item, match);
                        processed++;
                        continue;
                    }

                    match = regex2.Match(item.Title);
                    if (match.Success)
                    {
                        ProcessEpisode(ctx, item, match);
                        processed++;
                        continue;
                    }
                    else
                    {
                        Logging.PrintInvalid("Pattern match failed");
                    }
                }
            }

            Logging.PrintNewline();
            Logging.PrintStatus(ConsoleColor.DarkGreen, "# Item count", items.Count.ToString());
            Logging.PrintStatus(ConsoleColor.DarkGreen, "# Downloads", Downloads.ToString());
            Logging.PrintStatus(ConsoleColor.DarkGreen, "# Duplicates", processed.ToString());
            Logging.PrintStatus(ConsoleColor.DarkGreen, "# DateSkips", skipped.ToString());

            Logging.End();
        }

        private static void ProcessEpisode(MumsContext ctx, Episode item, Match match)
        {
            int season = int.Parse(match.Groups[1].Value);
            int episode = int.Parse(match.Groups[2].Value);
            int index = match.Groups[0].Index;
            string titlePart = item.Title.Substring(0, index);

            var split = titlePart
                .Split(new char[] { ' ', '[', ']' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Replace('"', ' '));

            titlePart = string.Join(" AND ", split);

            var matches = ctx.ExecuteStoreQuery<RssEpisodeItems>(
                "SELECT * FROM RssEpisodeItems WHERE Season={0} AND Episode={1} AND (ReleaseName={2} OR FREETEXT(ReleaseName, {3}))",
                season,
                episode,
                item.Title,
                titlePart
            ).AsQueryable();

            var entity = new RssEpisodeItems
            {
                Episode = episode,
                Season = season,
                ReleaseName = item.Title,
                PubDate = item.PubDate,
                Added = DateTime.Now,
                EnclosureUrl = item.TorrentUrl.ToString(),
                EnclosureLength = item.TorrentSize
            };
            
            var duplicate = matches.FirstOrDefault();

            if (duplicate != null)
            {
                if (entity.ReleaseName != duplicate.ReleaseName)
                {
                    entity.DuplicateOf = duplicate.RssEpisodeItemId;
                    Duplicates++;
                    Logging.PrintDuplicate(entity.ReleaseName);
                }
            }
            else
            {
                Logging.PrintDownloaded(entity.ReleaseName);
                Downloads++;
                entity.Download = true;
                Thread.Sleep(120 * 1000); // wait for the full-text index to refresh (120 seconds)
            }

            ctx.RssEpisodeItems.AddObject(entity);
            ctx.SaveChanges();
        }

        static List<Episode> GetItems()
        {
            var items = new List<Episode>();

            try
            {
                var client = new WebClient();
                string feedContent = client.DownloadString(Settings.Default.FeedUrl);
                
                using (var reader = new StringReader(feedContent))
                {
                    XElement warez = XElement.Load(reader);

                    foreach (var item in warez.Descendants("item"))
                    {
                        Uri torrentUrl;

                        XElement enclosure = item.Element("enclosure");
                        string link = enclosure.Attribute("url").Value;

                        if (!Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out torrentUrl))
                        {
                            Logging.PrintInvalid("Invalid torrent url\t" + link);
                            continue;
                        }

                        long length;
                        if (!long.TryParse(enclosure.Attribute("length").Value, out length))
                        {
                            Logging.PrintInvalid("Invalid torrent size\t" + enclosure.Attribute("length"));
                            continue;
                        }

                        items.Add(new Episode
                        {
                            Title = (item.Element("title").Value ?? string.Empty).Trim(),
                            PubDate = DateTime.Parse(item.Element("pubDate").Value),
                            TorrentUrl = torrentUrl,
                            TorrentSize = length
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.PrintStatus(ConsoleColor.Red, "Exception", "Exception in GetItems:\n" + ex.ToString());
            }

            return items;
        }
    }
}
