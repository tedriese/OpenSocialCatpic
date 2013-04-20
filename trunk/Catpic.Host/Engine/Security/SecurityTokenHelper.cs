using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Catpic.Gadgets.Security;

namespace Catpic.Host.Engine.Security
{
    /// <summary>
    /// Provides helper methods to security functionality
    /// </summary>
    internal static class SecurityTokenHelper
    {
        /// <summary>
        /// Returns token client state by principal
        /// </summary>
        public static string GetToken(IPrincipal pricipal)
        {
            if (pricipal.Identity.IsAuthenticated)
            {
                var user = pricipal as ICatpicPrincipal;
                var token = user.Token;
                return token.ToClientState();
            }

            return "john.doe:john.doe:appid:cont:url:0:default";
        }
    }
}