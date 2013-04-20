using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Catpic.Host.Areas.Portal.Controllers
{
    public class AboutController: Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult License()
        {
            return View();
        }
    }
}