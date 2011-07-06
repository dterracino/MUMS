using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using MUMS.Utorrent.Model;

namespace MUMS.Web.Models
{
    [DataContract]
    public class Section
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public List<Torrent> Torrents { get; set; }

        [DataMember]
        public int Status { get; set; }

        [DataMember]
        public bool Finished { get; set; }
    }
}