using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Catpic.Host.Areas.Portal.Controllers
{
    /// <summary>
    /// Home page contoller
    /// </summary>
    public class HomeController : Controller
    {
        //
        // GET: /Portal/Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details()
        {
            return View();
        }


        public ActionResult Resources()
        {
            return View();
        }

    }
}
