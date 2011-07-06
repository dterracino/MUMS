using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MUMS.Web.Models
{
    [DataContract]
    public class RpxError
    {
        [DataMember(Name = "msg")]
        public string Message { get; set; }

        [DataMember(Name="code")]
        public int ErrorCode { get; set; }
    }
}