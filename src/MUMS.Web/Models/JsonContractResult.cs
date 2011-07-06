using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Mvc;

namespace MUMS.Models
{
    public class JsonContractResult: ActionResult
    {
        public JsonContractResult(Object data)
        {
            this.Data = data;
        }

        public Object Data { get; private set; }

        public override void ExecuteResult(ControllerContext context)
        {
            var serializer = new DataContractJsonSerializer(this.Data.GetType());
            var output = String.Empty;

            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, this.Data);
                output = Encoding.UTF8.GetString(ms.ToArray());
            }

            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.Write(output);
        }
    }
}