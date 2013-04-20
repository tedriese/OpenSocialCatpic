using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Catpic.Host.Areas.Social.Controllers
{
    public class MessagesController : Controller
    {
        //
        // GET: /Social/Messages/

        public ActionResult Index()
        {
            return View();
        }

    }
}
