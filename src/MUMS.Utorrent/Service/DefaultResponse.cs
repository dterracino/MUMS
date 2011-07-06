using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MUMS.Utorrent.Service
{
    [DataContract(Namespace = "")]
    public class DefaultResponse
    {
        [DataMember(Name = "build", Order = 1)]
        public int Build { get; set; }
    }
}
