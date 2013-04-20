using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Catpic.Utils;
using Catpic.Utils.Diagnostic;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace Catpic.Host.Engine.Security
{
    /// <summary>
    /// Processes google authentication
    /// </summary>
    public class GoogleOpenIdRequestor : IOpenIdRequestor
    {
        public OutgoingWebResponse RedirectingResponse(IAuthenticationRequest request)
        {

            FetchRequest fetch = new FetchRequest();
            fetch.Attributes.Add(new AttributeRequest(WellKnownAttributes.Contact.Email, true));
            fetch.Attributes.Add(new AttributeRequest(WellKnownAttributes.Name.First, true));
            fetch.Attributes.Add(new AttributeRequest(WellKnownAttributes.Name.Last, true));
            fetch.Attributes.Add(new AttributeRequest(WellKnownAttributes.Preferences.Language, true));
            request.AddExtension(fetch);
            return request.RedirectingResponse;

        }

        public string CallbackResponse(IAuthenticationResponse response)
        {
            var fetches = response.GetExtension<FetchResponse>();

            var email = fetches.Attributes[WellKnownAttributes.Contact.Email].Values[0];
            FormsAuthentication.SetAuthCookie(email, false);
            return email;

        }
    }
}