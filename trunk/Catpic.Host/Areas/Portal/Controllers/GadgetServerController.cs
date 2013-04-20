using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using Catpic.Gadgets;
using Catpic.Gadgets.Security;
using Catpic.Host.Engine.Inline;
using Catpic.Host.Engine.Security;

namespace Catpic.Host.Areas.Portal.Controllers
{
    /// <summary>
    /// Controller of demo page which shows gadget server capabilities
    /// </summary>
    public class GadgetServerController : AsyncController
    {
        public const string DefaultGadget = "http://www.rareworksllc.com/moonphases/LunarPhaseGadget.xml";

        private readonly IRequestHandler _handler;
        private readonly InlineContextFactory _inlineFactory;
        //
        // GET: /Portal/Demo/

        public GadgetServerController(IRequestHandler handler, InlineContextFactory inlineFactory)
        {
            _handler = handler;
            _inlineFactory = inlineFactory;
        }

        public ActionResult Index()
        {
            ViewData["url"] = DefaultGadget;
            ViewData["token"] = SecurityTokenHelper.GetToken(User);
            return View();
        }

        [HttpPost]
        public ActionResult Container(string url)
        {
            ViewData["url"] = url;
            ViewData["token"] = SecurityTokenHelper.GetToken(User);
            return View();
        }

        [HttpGet]
        public ActionResult Container()
        {
            ViewData["url"] = DefaultGadget;
            ViewData["token"] = SecurityTokenHelper.GetToken(User);
            return View();
        }

        public ActionResult Inline()
        {
            return View();
        }

        public void RenderInlineAsync(string url, string mode)
        {
            //emulate query string
            NameValueCollection query = new NameValueCollection();
            query.Add("url", url);
            query.Add("renderType", mode);

            //emulate response output
            StringWriter writer = new StringWriter();

            var context = _inlineFactory.Create(query, writer,ControllerContext.HttpContext);
            AsyncManager.OutstandingOperations.Increment();
            _handler.CreateGadgetAsync(context)
                .ContinueWith(t =>
                {
                    AsyncManager.Parameters["content"] = writer.ToString();
                    AsyncManager.OutstandingOperations.Decrement();
                });
        }

        public ActionResult RenderInlineCompleted(string content)
        {
            return Content(content);
        }
    }
}
