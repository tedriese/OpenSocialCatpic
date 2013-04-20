using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Catpic.Host.Areas.Social
{
    /// <summary>
    /// OpenSocial area
    /// </summary>
    public class SocialAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "Social"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
             context.MapRoute(
                 "social_default",
                 "social/{controller}/{action}/{id}",
                 new {action = "Index", id = UrlParameter.Optional}
                 );
        }
    }
}