// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecurityRequestHandler.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents security handler
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Security
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Threading.Tasks;

    using Catpic.Gadgets.Proxies;

    /// <summary>
    /// Represents security handler
    /// </summary>
    public class SecurityRequestHandler : ISecurityRequestHandler
    {
        /// <summary>
        /// Oauth handlers
        /// </summary>
        private readonly IEnumerable<IOAuthRequestHandler> _oauthHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityRequestHandler"/> class.
        /// </summary>
        /// <param name="oauthHandlers"> Oauth handlers. </param>
        public SecurityRequestHandler(IEnumerable<IOAuthRequestHandler> oauthHandlers)
        {
            this._oauthHandlers = oauthHandlers;
        }

        /// <summary>
        /// Processes security calls and calls original handler
        /// </summary>
        /// <param name="context"> Proxy context. </param>
        /// <param name="originalHandler"> Original Handler. </param>
        /// <returns> Async task. </returns>
        public Task ProcessRequest(ProxyContext context, Func<ProxyContext, IDictionary<string, string>, string,  Task> originalHandler)
        {
            var token = context.SecurityToken as OAuthSecurityToken;
            
            // no request and access tokens
            if (token != null && token.Token == null)
            {
                return this.GetHandler(context, token).ProcessRequestToken(context, token);
            }

            // execute original handler
            var headers = this.GetAuthHeaders(context, token);
            var qs = this.GetAuthQueryString(context, token);
            return originalHandler(context, headers, qs);
        }

        /// <summary>
        /// Processes security callback
        /// </summary>
        /// <param name="context"> Proxy context. </param>
        /// <returns> Async task. </returns>
        public Task ProcessCallback(ProxyContext context)
        {
            return this.GetHandler(context, null).ProcessCallback(context);
        }

        /// <summary>
        /// Gets auth headers
        /// </summary>
        /// <param name="context"> Proxy context. </param>
        /// <param name="token"> Security token. </param>
        /// <returns> Auth headers map</returns>
        private Dictionary<string, string> GetAuthHeaders(ProxyContext context, OAuthSecurityToken token)
        {
            var request = context.Http.Request;
            NameValueCollection qs = request.HttpMethod == "GET" ? request.QueryString : request.Form;
            var requestUri = new Uri(qs["url"]);
            var method = qs["httpMethod"];

            if (token == null)
            {
                return null;
            }

            return this.GetHandler(context, token).GetAuthHeaders(token, requestUri, method);
        }

        /// <summary>
        /// Gets auth query string
        /// </summary>
        /// <param name="context"> Proxy context. </param>
        /// <param name="token"> Security token. </param>
        /// <returns> Auth headers map</returns>
        private string GetAuthQueryString(ProxyContext context, OAuthSecurityToken token)
        {
            var request = context.Http.Request;
            NameValueCollection qs = request.HttpMethod == "GET" ? request.QueryString : request.Form;
            var requestUri = new Uri(qs["url"]);
            var method = qs["httpMethod"];

            if (token == null)
            {
                return null;
            }

            return this.GetHandler(context, token).GetAuthQueryString(token, requestUri, method);
        }

        /// <summary>
        /// Gets oauth request handler
        /// </summary>
        /// <param name="context"> Proxy context. </param>
        /// <param name="token"> Security token. </param>
        /// <returns> Oauth request handler</returns>
        private IOAuthRequestHandler GetHandler(ProxyContext context, OAuthSecurityToken token)
        {
            // NOTE: use First instead to allow multiply handler registrations?
            return this._oauthHandlers.Single(o => o.CanHandle(context, token));
        }
    }
}
