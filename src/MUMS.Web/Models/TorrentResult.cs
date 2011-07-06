using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MUMS.Web.Models
{
    [DataContract]
    public class TorrentResult
    {
        [DataMember]
        public bool Ok { get; set; }
        
        [DataMember]
        public string Hash { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    }
}