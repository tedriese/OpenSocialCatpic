// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecurityTokenFactory.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Provides base functionality to create security tokens from http context
//   NOTE: if you don't want to allow anonymous access just override CreateAnonymous method in derived class
//   and throw specific exception
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Security
{
    using System;
    using System.Collections.Generic;
    using System.Security.Principal;
    using System.Web;

    using Catpic.Utils;
    using Catpic.Utils.Caching;
    using Catpic.Utils.Diagnostic;
    using Catpic.Utils.OAuth;

    /// <summary>
    /// Provides base functionality to create security tokens from http context
    /// NOTE: if you don't want to allow anonymous access just override CreateAnonymous method in derived class
    /// and throw specific exception
    /// </summary>
    public class SecurityTokenFactory : ISecurityTokenFactory
    {
        #region Consts

        /// <summary>
        /// none auth type in request
        /// </summary>
        private const string NoneType = "NONE";

        /// <summary>
        /// oauth1.0a auth type in request
        /// </summary>
        private const string OAuthType = "OAUTH";

        /// <summary>
        /// oauth2 auth type in request
        /// </summary>
        private const string OAuth2Type = "OAUTH2";

        /// <summary>
        /// signed request in request
        /// </summary>
        private const string SignedType = "SIGNED";

        /// <summary>
        /// Gadget parameter in request
        /// </summary>
        private const string GadgetParam = "gadget";

        /// <summary>
        /// Token parameter in request
        /// </summary>
        private const string TokenParam = "st";

        #endregion

        /// <summary>
        /// Trace category
        /// </summary>
        private const string TraceCategory = "token.factory";

        /// <summary>
        /// Trace instance
        /// </summary>
        private static readonly ITrace Trace = TraceFactory.GetTrace();

        /// <summary>
        /// Crypto service
        /// </summary>
        private readonly ICryptoService _cryptoService;

        /// <summary>
        /// Cache instance
        /// </summary>
        private readonly ICache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityTokenFactory"/> class.
        /// </summary>
        /// <param name="cryptoService"> Crypto service. </param>
        /// <param name="cacheResolver"> Cache resolver. </param>
        public SecurityTokenFactory(ICryptoService cryptoService, Func<string, ICache> cacheResolver)
        {
            this._cryptoService = cryptoService;
            this._cache = cacheResolver(GadgetConsts.TokenCache);
        }

        #region Create members

        /// <summary>
        ///  Creates token from properties map
        /// </summary>
        /// <param name="user"> The user. </param>
        /// <param name="properties"> Property map. </param>
        /// <returns> Security token </returns>
        public virtual ISecurityToken Create(IPrincipal user, IDictionary<string, object> properties)
        {
            string gadget = properties.SafeGet(GadgetParam, string.Empty);
            string token = properties.SafeGet(TokenParam, string.Empty);
            if (!user.Identity.IsAuthenticated)
            {
                return this.CreateAnonymous(gadget, token);
            }

            string name = user.Identity.Name;
            return this.GetToken(name, gadget, token);
        }

        /// <summary>
        /// Creates token from http request
        /// </summary>
        /// <param name="httpContext"> Http Context. </param>
        /// <returns> Security token. </returns>
        public virtual ISecurityToken Create(HttpContextBase httpContext)
        {
            // try to find token on context
            if (httpContext.User is ICatpicPrincipal)
            {
                var user = httpContext.User as ICatpicPrincipal;
                return user.Token;
            }
            
            // no token in context
            // is signed request?
            if (this.IsSignedRequest(httpContext))
            {
                return this.GetOAuthToken(httpContext);
            }

            // base or anonymous
            return this.GetToken(httpContext);
        }

        #endregion

        #region Anonymous token

        /// <summary>
        /// Creates anonymous token on behalf of john doe (or another person in case of valid st token in shindig format
        /// NOTE: override in derived class to disable anonymous access
        /// </summary>
        /// <param name="httpContext"> The http Context. </param>
        /// <returns> The create anonymous. </returns>
        public virtual ISecurityToken CreateAnonymous(HttpContextBase httpContext)
        {
            if (this.IsSignedRequest(httpContext))
            {
                return this.GetOAuthToken(httpContext);
            }

            string st = httpContext.Request.Params[TokenParam];
            string gadget = httpContext.Request.Params[GadgetParam];

            return this.CreateAnonymous(gadget, st);
        }

        #endregion

        /// <summary>
        /// Gets token from context
        /// </summary>
        /// <param name="httpContext"> Http Context. </param>
        /// <returns> Security token. </returns>
        protected virtual ISecurityToken GetToken(HttpContextBase httpContext)
        {
            string st = httpContext.Request.Params[TokenParam];
            string gadget = httpContext.Request.Params[GadgetParam];

            if (httpContext.User == null || !httpContext.User.Identity.IsAuthenticated)
            {
                return this.CreateAnonymous(gadget, st);
            }

            string name = httpContext.User.Identity.Name;
            return this.GetToken(name, gadget, st);
        }

        #region Signed request logic

        /// <summary>
        /// Gets oauth token
        /// </summary>
        /// <param name="httpContext"> Http context. </param>
        /// <returns> Security token</returns>
        protected ISecurityToken GetOAuthToken(HttpContextBase httpContext)
        {
            var state = httpContext.Request.Params["oauthState"];

            // client state keeps request token which was stored in cache
            if (!string.IsNullOrEmpty(state))
            {
                return this._cache.Get(state) as OAuthSecurityToken; // 2. oauth token has request token
            }

            // var st = httpContext.Request.Params[TokenParam]; 
            var gadget = httpContext.Request.Params[GadgetParam];

            // Token is present 
            ISecurityToken token = this.GetToken(httpContext);

            // 3. access token already verified
            var key = OAuthHelper.GetCacheKey(gadget, token.OwnerId, string.Empty);
            var cached = this._cache.Get(key);
            if (cached != null && cached is OAuthSecurityToken)
            {
                // 4. oauth token has access token
                return cached as OAuthSecurityToken;
            }

            // 1. no request or access tokens
            var authzType = httpContext.Request.Params.Get("authz").ToUpperInvariant();
            return new OAuthSecurityToken(
                token.OwnerId,
                token.OwnerId,
                string.Empty,
                string.Empty,
                gadget,
                string.Empty,
                string.Empty,
                authzType == OAuthType ? AuthType.OAuth : AuthType.OAuth2,
                this._cryptoService);
        }

        /// <summary>
        /// Tests whether request is signed using oauth 1.0 spec
        /// </summary>
        /// <param name="httpContext"> Http context. </param>
        /// <returns> True if Is request signed. </returns>
        private bool IsSignedRequest(HttpContextBase httpContext)
        {
            // oauth
            var authzType = httpContext.Request.Params.Get("authz");
            if (authzType != null)
            {
                authzType = authzType.ToUpperInvariant();
            }

            // TODO add oauth2
            if (authzType == OAuthType || authzType == OAuth2Type || authzType == SignedType)
            {
                return true;
            }

            return false;
        }
        #endregion

        #region Private members

        /// <summary>
        /// Creates anonymous token
        /// </summary>
        /// <param name="gadget"> Gadget id. </param>
        /// <param name="st"> Serialized security token . </param>
        /// <returns> Security token </returns>
        private ISecurityToken CreateAnonymous(string gadget, string st)
        {
            if (string.IsNullOrEmpty(st))
            {
                return this.GetToken(GadgetConsts.AnonymousName, gadget);
            }

            var parts = st.Split(':');

            // NOTE: expected count of token parts
            if (parts.Length != 7)
            {
                return this.GetToken(GadgetConsts.AnonymousName, gadget);
            }

            // NOTE: it is shindig's basic token format
            return new BasicSecurityToken(parts[0], parts[1], parts[2], parts[3], parts[4], parts[5], parts[6], this._cryptoService);
        }

        /// <summary>
        /// Gets Security token
        /// </summary>
        /// <param name="name"> User name. </param>
        /// <param name="gadget"> Gadget id. </param>
        /// <param name="st"> Serialized security token . </param>
        /// <returns> Security token. </returns>
        private ISecurityToken GetToken(string name, string gadget, string st)
        {
            BasicSecurityToken token = this.GetToken(name, gadget);

            // TODO case of corrupted token
            if (!string.IsNullOrEmpty(st))
            {
                try
                {
                    token.FromClientState(st);
                }
                catch (Exception ex)
                {
                    Trace.Warn(TraceCategory, string.Format("unable to extract client state: {0}", ex.Message));
                }
            }

            return token;
        }

        /// <summary>
        /// Gets Security token
        /// </summary>
        /// <param name="name"> User name. </param>
        /// <param name="gadget"> Gadget id. </param>
        /// <returns> Security token. </returns>
        private BasicSecurityToken GetToken(string name, string gadget)
        {
            // NOTE what is the difference between owner and viewer?
            return new BasicSecurityToken(name, name, gadget, string.Empty, string.Empty, string.Empty, string.Empty, this._cryptoService);
        }

        #endregion
    }
}
