// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOAuthRequestHandler.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Defines oauth request handler behavior
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Security
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Catpic.Gadgets.Proxies;

    /// <summary>
    /// Defines oauth request handler behavior
    /// </summary>
    public interface IOAuthRequestHandler
    {
        /// <summary>
        /// Processes request token.
        /// </summary>
        /// <param name="context"> Proxy context. </param>
        /// <param name="token"> Oauth token. </param>
        /// <returns>Async task </returns>
        Task ProcessRequestToken(ProxyContext context, OAuthSecurityToken token);

        /// <summary>
        /// Processes access token.
        /// </summary>
        /// <param name="context"> Proxy context. </param>
        /// <param name="token"> Security token. </param>
        /// <param name="verifier"> Verifier code. </param>
        /// <returns> Async task </returns>
        Task<OAuthSecurityToken> ProcessAccessToken(ProxyContext context, OAuthSecurityToken token, string verifier);

        /// <summary>
        /// Processes oauth callback
        /// </summary>
        /// <param name="context"> Proxy context. </param>
        /// <returns> Async task</returns>
        Task ProcessCallback(ProxyContext context);

        /// <summary>
        /// Gets auth headers
        /// </summary>
        /// <param name="token"> Security token. </param>
        /// <param name="requestUri"> Request uri. </param>
        /// <param name="method"> Http method. </param>
        /// <returns> Headers map</returns>
        Dictionary<string, string> GetAuthHeaders(OAuthSecurityToken token, Uri requestUri, string method);

        /// <summary>
        /// Gets auth query string
        /// </summary>
        /// <param name="token"> Security token. </param>
        /// <param name="requestUri"> Request uri. </param>
        /// <param name="method"> Http method. </param>
        /// <returns> Query string</returns>
        string GetAuthQueryString(OAuthSecurityToken token, Uri requestUri, string method);

        /// <summary>
        /// True, whether request handler can process request. Used to distinguish different oauth versions
        /// </summary>
        /// <param name="context"> Proxy context. </param>
        /// <param name="token"> Security token. </param>
        /// <returns> The can handle. </returns>
        bool CanHandle(ProxyContext context, OAuthSecurityToken token);
    }
}
