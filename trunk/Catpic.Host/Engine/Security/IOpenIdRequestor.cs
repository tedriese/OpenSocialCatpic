using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace Catpic.Host.Engine.Security
{
    /// <summary>
    /// Requests openid authentication
    /// </summary>
    public interface IOpenIdRequestor
    {
        /// <summary>
        /// Processes initial request
        /// </summary>
        OutgoingWebResponse RedirectingResponse(IAuthenticationRequest request);

        /// <summary>
        /// Processes provider's callback
        /// </summary>
        string CallbackResponse(IAuthenticationResponse response);
    }
}