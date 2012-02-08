using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MUMS.RssEpisodeFilter
{
    public class Episode
    {
        public string Title { get; set; }
        public DateTime PubDate { get; set; }
        public Uri TorrentUrl { get; set; }
        public long TorrentSize { get; set; }
        public Uri SourceUrl { get; set; }
    }
}
