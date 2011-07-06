using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MUMS.Web.Models
{
    [DataContract]
    public class RpxProfile
    {
        [DataMember(Name = "identifier")]
        public string Identifier { get; set; }
    }
}