using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Catpic.Host.Engine
{
    /// <summary>
    /// Configures filters
    /// </summary>
    public class FilterConfigurator
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new RequireAuthenticationAttribute());
        }

    }
}