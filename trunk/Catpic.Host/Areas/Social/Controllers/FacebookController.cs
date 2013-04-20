using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Catpic.Host.Engine.Security;

namespace Catpic.Host.Areas.Social.Controllers
{
    public class FacebookController : Controller
    {
        //
        // GET: /Social/Facebook/

        public ActionResult Index()
        {
            ViewData["url"] = "~/content/gadgets/oauth2/oauth2_facebook.xml";
            ViewData["token"] = SecurityTokenHelper.GetToken(User);
            
            return View();
        }

    }
}
