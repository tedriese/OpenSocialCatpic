using System;
using System.Web.Security;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace Catpic.Host.Engine.Security
{
    /// <summary>
    /// Processes myopenid authentication
    /// </summary>
    public class MyOpenIdRequestor : IOpenIdRequestor
    {
        public OutgoingWebResponse RedirectingResponse(IAuthenticationRequest request)
        {
            ClaimsRequest claim = new ClaimsRequest();

            claim.BirthDate = DemandLevel.Require;
            claim.Country = DemandLevel.Require;
            claim.Email = DemandLevel.Require;
            claim.FullName = DemandLevel.Require;
            claim.Gender = DemandLevel.Require;
            claim.Language = DemandLevel.Require;
            claim.Nickname = DemandLevel.Require;
            claim.PostalCode = DemandLevel.Require;
            claim.TimeZone = DemandLevel.Require;

            request.AddExtension(claim);
            return request.RedirectingResponse;

        }

        public string CallbackResponse(IAuthenticationResponse response)
        {
            //var claims = response.GetExtension<ClaimsResponse>();
            var displayName = response.FriendlyIdentifierForDisplay;
            FormsAuthentication.SetAuthCookie(displayName, false);
            return displayName;
        }
    }
}