using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MUMS.Web.Models;

namespace MUMS.Web.Controllers
{
    public partial class MumsController : Controller
    {
        public static int Version { get { return 7; } }
        public static string VersionParam { get { return "?v=" + Version; } }

        public virtual ActionResult JsonContract(object obj)
        {
            return new JsonContractResult(obj);
        }
    }
}