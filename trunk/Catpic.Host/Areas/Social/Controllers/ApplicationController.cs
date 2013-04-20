using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Catpic.Host.Areas.Social.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}
