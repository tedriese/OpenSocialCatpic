// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OAuth2RequestHandler.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Default oauth request handler. Supports OAuth2
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Security.OAuth2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Catpic.Gadgets.Format.OAuth2;
    using Catpic.Gadgets.Proxies;
    using Catpic.Utils;
    using Catpic.Utils.Caching;
    using Catpic.Utils.Diagnostic;
    using Catpic.Utils.OAuth;

    using Newtonsoft.Json;

    /// <summary>
    /// Default oauth request handler. Supports OAuth2 
    /// </summary>
    public class OAuth2RequestHandler : IOAuthRequestHandler
    {
        /// <summary>
        /// Trace category
        /// </summary>
        private const string TraceCategory = "oauth2.handler";

        /// <summary>
        /// Trace instance
        /// </summary>
        private static readonly ITrace Trace = TraceFactory.GetTrace();

        /// <summary>
        /// Cache instance
        /// </summary>
        private readonly ICache _cache;

        /// <summary>
        /// Gadget definition factory
        /// </summary>
        private readonly GadgetDefinitionFactory _gadgetFactory;

        /// <summary>
        /// Oauth consumer provider
        /// </summary>
        private readonly IOAuthConsumerProvider _consumerProvider;

        /// <summary>
        /// Oauth callback url
        /// </summary>
        private readonly string _callback;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2RequestHandler"/> class.
        /// </summary>
        /// <param name="gadgetFactory"> Gadget definition factory. </param>
        /// <param name="consumerProvider"> Consumer provider. </param>
        /// <param name="callback"> Callback url. </param>
        /// <param name="cacheResolver"> Cache resolver. </param>
        public OAuth2RequestHandler(
            GadgetDefinitionFactory gadgetFactory,
            IOAuthConsumerProvider consumerProvider,
            string callback,
            Func<string, ICache> cacheResolver)
        {
            this._gadgetFactory = gadgetFactory;
            this._consumerProvider = consumerProvider;
            this._callback = callback;
            this._cache = cacheResolver(GadgetConsts.TokenCache);
        }

        /// <summary>
        /// Builds response with approve url.
        /// </summary>
        /// <param name="context">Proxy context.</param>
        /// <param name="token">Security token.</param>
        /// <returns>Async task.</returns>
        public Task ProcessRequestToken(ProxyContext context, OAuthSecurityToken token)
        {
            var key = this.GetCacheKey(token);
            this._cache.Add(key, token);

            var resourceUri = context.Http.Request.Form["url"];
            var service = this.GetService(token);
            var approvalUri = this.GetManager(token, service).GetRequestUri(
                service.Authorization.Endpoint, this._callback, service.Scope, key);
           
            return AsyncHelper.GetEmptyTask().ContinueWith(t =>
                {
                    var clientState = key;
                    var jsonResponse = JsonConvert.SerializeObject(
                        new
                            {
                                rc = 200, 
                                body = string.Empty, 
                                oauthState = clientState, 
                                oauthApprovalUrl = approvalUri
                            });
                    context.Http.Response.Output.Write(
                        string.Format(
                            "{0}{{\"{1}\":{2}}}", 
                            GadgetConsts.UnparseableCruft, 
                            resourceUri, 
                            jsonResponse));
                });
        }

        /// <summary>
        /// Processes callback of service provider.
        /// </summary>
        /// <param name="context"> Proxy context. </param>
        /// <returns> Async task. </returns>
        public Task ProcessCallback(ProxyContext context)
        {
            var state = context.Http.Request["state"];
            var code = context.Http.Request["code"];
            var token = this._cache.Get(state) as OAuthSecurityToken;

            return this.ProcessAccessToken(context, token, code);
        }

        /// <summary>
        /// Gets access token.
        /// </summary>
        /// <param name="context"> Proxy context. </param>
        /// <param name="token"> Security token. </param>
        /// <param name="verifier"> Verifier code. </param>
        /// <returns> Async task. </returns>
        public Task<OAuthSecurityToken> ProcessAccessToken(ProxyContext context, OAuthSecurityToken token, string verifier)
        {
            var service = this.GetService(token);
            var oauthMng = this.GetManager(token, service);
            return
                oauthMng.AcquireAccessToken(service.Access.Endpoint, service.Access.Method, this._callback, verifier).
                    ContinueWith(
                        t =>
                            {
                                try
                                {
                                    var response = t.Result;
                                    token.Token = response["access_token"];

                                    // TODO process all response parameters, e.g.:
                                    /*{
                                            "access_token" : "ya29.AHES6ZSaaQbH4rKWX_XuOyGafCxfhrvithaiiFGGOHQNqk8",
                                            "token_type" : "Bearer",
                                            "expires_in" : 3599
                                        }*/
                                    token.IsAccessToken = true;
                                    var key = OAuthHelper.GetCacheKey(token.AppUrl, token.OwnerId, string.Empty);
                                    _cache.Add(key, token);
                                    return token;
                                }
                                catch (Exception ex)
                                {
                                    Trace.Error(TraceCategory, "Unable to get request token", ex);
                                    throw;
                                }
                            });
        }

        /// <summary>
        /// Gets auth headers (NOTE: not implemented)
        /// </summary>
        /// <param name="token"> Security token. </param>
        /// <param name="requestUri"> Request uri. </param>
        /// <param name="method"> Http method. </param>
        /// <returns> Auth headers map</returns>
        public Dictionary<string, string> GetAuthHeaders(OAuthSecurityToken token, Uri requestUri, string method)
        {
            // TODO
            return null;
        }

        /// <summary>
        ///  Gets auth query string
        /// </summary>
        /// <param name="token"> Security token. </param>
        /// <param name="requestUri"> Request uri. </param>
        /// <param name="method"> Http method. </param>
        /// <returns> Query string</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if token isn't access
        /// </exception>
        public string GetAuthQueryString(OAuthSecurityToken token, Uri requestUri, string method)
        {
            if (!token.IsAccessToken)
            {
                throw new ArgumentException("Token isn't access token");
            }
            
            return string.Format("access_token={0}", token.Token);
        }

        /// <summary>
        /// True, if request is oauth2
        /// </summary>
        /// <param name="context"> Proxy context. </param>
        /// <param name="token"> Security token. </param>
        /// <returns> true, if request can be processed</returns>
        public bool CanHandle(ProxyContext context, OAuthSecurityToken token)
        {
            return token != null ? token.Type == AuthType.OAuth2 : !string.IsNullOrEmpty(context.Http.Request["code"]);
        }

        #region Private memebrs

        /// <summary>
        /// Returns cache key.
        /// </summary>
        /// <param name="token"> Security token. </param>
        /// <returns> Cache key.</returns>
        private string GetCacheKey(OAuthSecurityToken token)
        {
            return OAuthHelper.Hash(string.Format("{0}.{1}", token.OwnerId, token.AppUrl));
        }

        /// <summary>
        /// Gets oauth2 manager
        /// </summary>
        /// <param name="token"> Security token. </param>
        /// <param name="service"> Oauth2 service description. </param>
        /// <returns> Oauth2 manager</returns>
        private OAuth2Manager GetManager(OAuthSecurityToken token, Service2Definition service = null)
        {
            if (service == null)
            {
                service = this.GetService(token);
            }

            var consumer = this._consumerProvider.GetOAuth2(token.AppUrl, service.Name);

            return new OAuth2Manager(consumer.ClientId, consumer.Secret);
        }

        /// <summary>
        /// Gets oauth2 service description
        /// </summary>
        /// <param name="token"> Security token. </param>
        /// <returns> Oauth2 service description</returns>
        private Service2Definition GetService(OAuthSecurityToken token)
        {
            var resolvedUri = FileHelper.ResolvePath(token.AppUrl);
            var uri = new Uri(resolvedUri);
            var gadget = this._gadgetFactory.Get(uri);

            // NOTE: which service should be used?
            return gadget.ModulePreferences.OAuth2.Services.First();
        }

        #endregion
    }
}
