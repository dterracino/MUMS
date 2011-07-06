using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MUMS.Web.Controllers
{
    public partial class ContentController : MumsController
    {
        public virtual ActionResult Stylesheet(string stylesheetPath)
        {
            return Content(string.Format("<link href=\"{0}{1}\" rel=\"stylesheet\" />", stylesheetPath, MumsController.VersionParam));
        }

        public virtual ActionResult Script(string scriptPath)
        {
            return Content(string.Format("<script type=\"text/javascript\" src=\"{0}{1}\"></script>", scriptPath, MumsController.VersionParam));
        }
    }
}