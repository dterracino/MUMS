using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MUMS.Web.Models
{
    [DataContract]
    public class PollTorrentsModel
    {
        [DataMember]
        public List<Section> Sections { get; set; }

        [DataMember]
        public long DownloadSpeedInBytes { get; set; }
        
        [DataMember]
        public long UploadSpeedInBytes { get; set; }
    }
}