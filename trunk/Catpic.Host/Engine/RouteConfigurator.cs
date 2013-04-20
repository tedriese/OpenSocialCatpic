using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Catpic.Host.Engine
{
    /// <summary>
    /// Configures gadget server and open social routing system
    /// </summary>
    public class RouteConfigurator
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{*staticfile}", new { staticfile = @".*\.(gif|jpg)(/.*)?" });

            var route = routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
                );
            route.DataTokens["area"] = "Portal";

        }
    }
}