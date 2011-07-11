using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Mvc;
using MUMS.Web;

namespace MUMS.Web.Models
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
            string output = JsonSerializer.ToJson(Data);
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.Write(output);
        }
    }
}