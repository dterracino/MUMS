using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MUMS.Web.Models
{
    [DataContract]
    public class Section
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public List<TorrentModel> Torrents { get; set; }

        [DataMember]
        public int Status { get; set; }

        [DataMember]
        public bool Finished { get; set; }
    }
}