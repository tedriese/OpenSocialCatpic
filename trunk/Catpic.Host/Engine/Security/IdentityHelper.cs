using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Catpic.Host.Engine.Security
{
    using Catpic.Web.Security;

    public static class IdentityHelper
    {
        /// <summary>
        /// True if request is authenticated and user isn't anonymous, see Global.asax for details
        /// </summary>
        public static bool IsAuthenticated(IPrincipal user, HttpRequestBase request)
        {
            return request.IsAuthenticated &&
                    ((user is CatpicPrincipal) && (user as CatpicPrincipal).Token.ViewerId != Consts.AnonymousId);
        }
    }
}