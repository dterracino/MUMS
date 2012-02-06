using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MUMS.Web.Models
{
    [DataContract]
    public class PollEpisodesModel
    {
        [DataMember]
        public List<RssEpisodeModel> LatestEpisodes { get; set; }
    }
}