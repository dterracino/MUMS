using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace MUMS.Web.Models
{
    [DataContract]
    public class RpxResponse
    {
        private static DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(RpxResponse));

        [DataMember(Name = "stat")]
        public string Status { get; set; }

        [DataMember(Name = "err")]
        public RpxError Error { get; set; }

        [DataMember(Name = "profile")]
        public RpxProfile Profile { get; set; }

        public static RpxResponse Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
                throw new ArgumentNullException("json");

            return Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(json)));
        }

        public static RpxResponse Deserialize(Stream jsonStream)
        {
            if (jsonStream == null)
            {
                throw new ArgumentNullException("jsonStream");
            }

            return (RpxResponse)jsonSerializer.ReadObject(jsonStream);
        }
    }
}