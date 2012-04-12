using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using MUMS.Data;
using MUMS.RssEpisodeFilter.Extensions;

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
        /// Matches "Foo.109.bar" with an optional x in the middle: "Foo.1x09.bar".
        /// </summary>
        static Regex regex2 = new Regex(@"\.([0-9])(x?)([0-9]+)\.", RegexOptions.IgnoreCase);

        /// <summary>
        /// Matches (the pretty unusual) variant "Foo 1x9 bar".
        /// </summary>
        static Regex regex3 = new Regex(@"\ ([0-9])x([0-9]+)", RegexOptions.IgnoreCase);

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

                foreach (ParsedEpisode item in items)
                {
                    bool skip = false;
                    if (skipDate && item.PubDate < currentMaxDate)
                    {
                        skip = true;
                        skipped++;
                        Logging.PrintDateSkipped(item.ShowName);
                    }

                    if (SetSeasonAndEpisode(item))
                    {
                        ProcessEpisode(ctx, item, skip);
                        processed++;
                    }
                    else
                    {
                        Logging.PrintInvalid("Pattern match failed: " + item.ShowName);
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

        private static bool SetSeasonAndEpisode(ParsedEpisode item)
        {
            Match match = regex1.Match(item.ReleaseName);

            if (match.Success)
            {
                SetSeasonAndEpisode(item, match, 1, 3);
                return true;
            }

            match = regex2.Match(item.ReleaseName);
            if (match.Success)
            {
                SetSeasonAndEpisode(item, match, 1, 3);
                return true;
            }

            match = regex3.Match(item.ReleaseName);
            if (match.Success)
            {
                SetSeasonAndEpisode(item, match, 1, 2);
                return true;
            }

            return false;
        }

        private static void SetSeasonAndEpisode(ParsedEpisode item, Match match, int idxSeason, int idxEpisode)
        {
            item.ShowName = item.ReleaseName.Substring(0, match.Groups[0].Index).Trim();
            item.Season = int.Parse(match.Groups[idxSeason].Value);
            item.Episode = int.Parse(match.Groups[idxEpisode].Value);
        }

        private static void ProcessEpisode(MumsDataContext ctx, ParsedEpisode item, bool skip = false)
        {
            var split = item.ShowName.Trim()
                .Split(new char[] { ' ', '[', ']' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Replace('"', ' '));

            var matches = ctx.ExecuteStoreQuery<RssEpisodeItems>(
                "SELECT * FROM RssEpisodeItems WHERE Season={0} AND Episode={1} AND (ReleaseName={2} OR EnclosureUrl={3} OR FREETEXT(ReleaseName, {4}))",
                item.Season,
                item.Episode,
                item.ReleaseName,
                item.TorrentUrl.ToString(),
                string.Join(" AND ", split)
            ).AsQueryable();

            var entity = new RssEpisodeItems
            {
                Episode = item.Episode,
                Season = item.Season,
                ReleaseName = item.ReleaseName,
                PubDate = item.PubDate,
                Added = DateTime.Now,
                EnclosureUrl = item.TorrentUrl.ToString(),
                EnclosureLength = item.TorrentSize,
                SourceUrl = item.SourceUrl.ToString(),
                ShowName = item.ShowName.Replace('.', ' ').Trim()
            };

            var duplicate = matches.FirstOrDefault();

            if (duplicate != null)
            {
                // I am only interested in adding an item if there is no item with that releasename yet.
                // This is because the more duplicates of the same episode with different releasenames there are,
                // the greater the chance of identifying another duplicate.
                bool exists = ctx.RssEpisodeItems.Any(i => i.ReleaseName == entity.ReleaseName);
                Logging.PrintDuplicate(entity.ReleaseName);

                if (exists)
                    return;

                entity.DuplicateOf = duplicate.RssEpisodeItemId;
                Duplicates++;
            }
            else
            {
                Logging.PrintDownloaded(entity.ReleaseName);
                Downloads++;
                entity.Download = true;
            }

            if (skip)
                entity.Download = false;

            ctx.RssEpisodeItems.AddObject(entity);
            ctx.SaveChanges();

            if (entity.Download)
                Thread.Sleep(180 * 1000); // wait for the full-text index to refresh (180 seconds)
        }
    }
}
