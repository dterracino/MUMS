using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace MUMS.Web
{
    public static class JsonSerializer
    {
        public static string ToJson<T>(T entity)
        {
            var serializer = new DataContractJsonSerializer(entity.GetType());

            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, entity);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public static T FromJson<T>(string json)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                return (T)serializer.ReadObject(ms);
        }
    }
}