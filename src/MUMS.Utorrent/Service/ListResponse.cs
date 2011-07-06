using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MUMS.Utorrent.Service
{
    [DataContract]
    public class ListResponse
    {
        [DataMember(Name="label")]
        public StringList Labels { get; set; }

        [DataMember(Name="torrents")]
        public StringList Torrents { get; set; }
    }
}
