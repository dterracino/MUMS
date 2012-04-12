using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MUMS.RssEpisodeFilter
{
    public class ParsedEpisode
    {
        public string ShowName { get; set; }
        public string ReleaseName { get; set; }
        public DateTime PubDate { get; set; }
        public Uri TorrentUrl { get; set; }
        public long TorrentSize { get; set; }
        public Uri SourceUrl { get; set; }
        public int Season { get; set; }
        public int Episode { get; set; }
    }
}
