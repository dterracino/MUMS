using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;
using MUMS.Data;
using MUMS.RssEpisodeFilter.Extensions;
using MUMS.RssEpisodeFilter.Properties;

namespace MUMS.RssEpisodeFilter
{
    class Program
    {
        /// <summary>
        /// Matches "Foo.S01E03.bar" with an optional double episode match: "S01E03-04". There may also be
        /// a delimiter between season and episode: "Foo.S01_E03.bar" or "Foo.S01.E03.bar".
        /// </summary>
        static Regex regex1 = new Regex(@"S([0-9]{2})(\.|_)?E([0-9]{2})(-E[0-9]{2})?", RegexOptions.IgnoreCase);

        /// <summary>
        /// Matches "Foo.109.bar" with an optional x in the middle: "Foo.1x09.bar"
        /// </summary>
        static Regex regex2 = new Regex(@"\.([0-9])(x?)([0-9]+)\.", RegexOptions.IgnoreCase);

        static DateTime BatchDate = DateTime.Now;

        static int Downloads = 0;
        static int Duplicates = 0;

        static void Main(string[] args)
        {
            var items = ItemExtracter.GetItems();
            items.Shuffle();

            var currentMaxDate = DateTime.MinValue;
            Match match;

            int skipped = 0;
            int processed = 0;

            bool skipDate = false;
            if (args != null && args.Contains("/skip"))
                skipDate = true;

            using (var ctx = new MumsDataContext())
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
                        Logging.PrintInvalid("Pattern match failed: " + item.Title);
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

        private static void ProcessEpisode(MumsDataContext ctx, Episode item, Match match)
        {
            int season = int.Parse(match.Groups[1].Value);
            int episode = int.Parse(match.Groups[3].Value);
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
                EnclosureLength = item.TorrentSize,
                SourceUrl = item.SourceUrl.ToString()
            };

            var duplicate = matches.FirstOrDefault();

            if (duplicate != null)
            {
                bool exists = ctx.RssEpisodeItems.Any(i => i.ReleaseName == entity.ReleaseName);
                if (!exists)
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
                Thread.Sleep(180 * 1000); // wait for the full-text index to refresh (180 seconds)
            }

            ctx.RssEpisodeItems.AddObject(entity);
            ctx.SaveChanges();
        }
    }
}
