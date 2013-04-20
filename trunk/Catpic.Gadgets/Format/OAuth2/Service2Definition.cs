// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Service2Definition.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   A single OAuth 2.0 service configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Format.OAuth2
{
    /// <summary>
    /// A single OAuth 2.0 service configuration.
    /// </summary>
    public class Service2Definition
    {
        /// <summary>
        /// Gets or sets the name of the service, used for referencing OAuth 2.0 services at runtime. 
        /// This parameter is optional, and if unspecified defaults to an empty string. 
        /// Gadget developers specify which OAuth 2.0 service they wish to use by passing 
        /// the service name as a parameter to gadgets.io.makeRequest
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///  Gets or sets the value of the OAuth 2.0 scope parameter to be used by default in requests. 
        /// This value is superseded when gadgets.io.RequestParameters.OAUTH2_SCOPE is used. 
        /// Access Token Scope is defined in Section 3.3 of the OAuth 2.0
        ///  [draft-ietf-oauth-v2-22] specification. This parameter is OPTIONAL.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets the OAuth 2.0 authorization endpoint.
        /// </summary>
        public OAuth.TokenRequest Authorization { get; set; }

        /// <summary>
        /// Gets or sets the OAuth 2.0 token endpoint.
        /// </summary>
        public OAuth.TokenRequest Access { get; set; }
    }
}
