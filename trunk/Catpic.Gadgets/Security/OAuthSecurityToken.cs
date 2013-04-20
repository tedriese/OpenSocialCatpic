// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OAuthSecurityToken.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Extends basic token with OAuth-specific fields
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Security
{
    using System;

    using Catpic.Utils.OAuth;

    /// <summary>
    /// Extends basic token with OAuth-specific fields
    /// </summary>
    public class OAuthSecurityToken : BasicSecurityToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthSecurityToken"/> class.
        /// </summary>
        /// <param name="owner"> Owner id. </param>
        /// <param name="viewer"> Viewer id. </param>
        /// <param name="app"> Application id. </param>
        /// <param name="domain"> Domain name. </param>
        /// <param name="appUrl"> Application url. </param>
        /// <param name="moduleId"> Module id. </param>
        /// <param name="container"> Container ma,e. </param>
        /// <param name="type"> Token type. </param>
        /// <param name="cryptoService"> Crypto service. </param>
        public OAuthSecurityToken(
            string owner,
            string viewer,
            string app,
            string domain,
            string appUrl,
            string moduleId,
            string container,
            AuthType type,
            ICryptoService cryptoService)
            : base(owner, viewer, app, domain, appUrl, moduleId, container, cryptoService)
        {
            Type = type;
        }

        /// <summary>
        /// Gets or sets oauth token type
        /// </summary>
        public AuthType Type { get; set; }

        /// <summary>
        /// Gets or sets OAuth token: request or access
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets OAuth token secret: request or access
        /// </summary>
        public string TokenSecret { get; set; }

        /// <summary>
        /// Gets or sets expiration date of access token
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether token is access token.
        /// </summary>
        public bool IsAccessToken { get; set; }
    }
}
