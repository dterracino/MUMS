using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UTorrentAPI;

namespace MUMS.Web.Models
{
    public class DetailsModel: TorrentModel
    {
        public List<File> Files { get; set; }
        public bool Finished { get { return this.Percentage == 100; } }

        public DetailsModel()
        {
            Files = new List<File>();
        }

        public DetailsModel(Torrent torrent): base(torrent)
        {
            if (torrent.Files == null)
                Files = new List<File>();
            else
                Files = torrent.Files.ToList();
        }
    }
}