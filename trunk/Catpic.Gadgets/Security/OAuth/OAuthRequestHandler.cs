// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OAuthRequestHandler.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Default oauth request handler. Supports OAuth 1.0a
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Security.OAuth
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Catpic.Gadgets.Format.OAuth;
    using Catpic.Gadgets.Proxies;
    using Catpic.Utils;
    using Catpic.Utils.Caching;
    using Catpic.Utils.Diagnostic;
    using Catpic.Utils.OAuth;

    using Newtonsoft.Json;

    /// <summary>
    /// Default oauth request handler. Supports OAuth 1.0a 
    /// </summary>
    public class OAuthRequestHandler : IOAuthRequestHandler
    {
        /// <summary>
        /// Trace category
        /// </summary>
        private const string TraceCategory = "oauth.handler";

        /// <summary>
        /// Trace instance
        /// </summary>
        private static readonly ITrace Trace = TraceFactory.GetTrace();

        /// <summary>
        /// Cache instance
        /// </summary>
        private readonly ICache _cache;

        /// <summary>
        /// Gadget factory
        /// </summary>
        private readonly GadgetDefinitionFactory _gadgetFactory;

        /// <summary>
        /// Consumer provider
        /// </summary>
        private readonly IOAuthConsumerProvider _consumerProvider;

        /// <summary>
        /// Callback path
        /// </summary>
        private readonly string _callback;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthRequestHandler"/> class.
        /// </summary>
        /// <param name="gadgetFactory"> Gadget factory. </param>
        /// <param name="consumerProvider"> Consumer provider. </param>
        /// <param name="callback"> Callback path. </param>
        /// <param name="cacheResolver"> Cache resolver. </param>
        public OAuthRequestHandler(
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
        /// Processes request token.
        /// </summary>
        /// <param name="context"> Proxy context. </param>
        /// <param name="token"> Oauth token. </param>
        /// <returns>Async task </returns>
        public Task ProcessRequestToken(ProxyContext context, OAuthSecurityToken token)
        {
            ServiceDefinition service = this.GetService(token);
            OAuthManager oauth = this.GetManager(token, service.Name);
            oauth["callback"] = this._callback;
           
            // get original uri
            var resourceUri = context.Http.Request.Form["url"];
            
            // get uri and method from OAuth section
            var request = service.Request;
            var authorize = service.Authorization;
            return oauth.AcquireRequestToken(request.Endpoint.ToString(), request.Method).ContinueWith(t =>
            {
                try
                {
                    // TODO checking
                    var requestResponse = t.Result;
                    token.Token = requestResponse["oauth_token"];
                    token.TokenSecret = requestResponse["oauth_token_secret"];

                    _cache.Add(token.Token, token);

                    // TODO encrypt client state and pass as oauthstate param
                    var clientState = token.Token;

                    var authorizeUri = string.Format("{0}?oauth_token={1}", authorize, requestResponse["oauth_token"]);
                    var jsonResponse =
                        JsonConvert.SerializeObject(new
                        {
                            rc = 200,
                            body = string.Empty,
                            // st = "serializedToken",
                            oauthState = clientState,
                            oauthApprovalUrl = authorizeUri
                        });

                    context.Http.Response.Output.Write(
                        string.Format(
                            "{0}{{\"{1}\":{2}}}",
                            GadgetConsts.UnparseableCruft,
                            resourceUri,
                            jsonResponse));
                }
                catch (Exception ex)
                {
                    Trace.Error(TraceCategory, "Unable to get request token", ex);
                }
            });
        }

        /// <summary>
        /// Processes access token.
        /// </summary>
        /// <param name="context"> Proxy context. </param>
        /// <param name="token"> Security token. </param>
        /// <param name="verifier"> Verifier code. </param>
        /// <returns> Async task </returns>
        public Task<OAuthSecurityToken> ProcessAccessToken(ProxyContext context, OAuthSecurityToken token, string verifier = "")
        {
            ServiceDefinition service = this.GetService(token);
            OAuthManager oauth = this.GetManager(token, service.Name);
            oauth["token"] = token.Token;
            oauth["token_secret"] = token.TokenSecret;

            var access = service.Access;
            return oauth.AcquireAccessToken(access.Endpoint.ToString(), access.Method, verifier).ContinueWith(t =>
            {
                try
                {
                    // TODO checking
                    var accessResponse = t.Result;
                    token.Token = accessResponse["oauth_token"];
                    token.TokenSecret = accessResponse["oauth_token_secret"];
                    token.IsAccessToken = true;

                    // update cache entry with verified access token instead of request
                    // TODO pass serviceName here when token factory supports it
                    var key = OAuthHelper.GetCacheKey(token.AppUrl, token.OwnerId, string.Empty);
                    _cache.Add(key, token);

                    return token;
                }
                catch (Exception ex)
                {
                    Trace.Error(TraceCategory, "Unable to get access token", ex);
                    return null;
                }
            });
        }

        /// <summary>
        /// Processes oauth callback
        /// </summary>
        /// <param name="context"> Proxy context. </param>
        /// <returns> Async task</returns>
        public Task ProcessCallback(ProxyContext context)
        {
            var requestToken = context.Http.Request["oauth_token"];
            var verifier = context.Http.Request["oauth_verifier"];
            var token = this._cache.Get(requestToken) as OAuthSecurityToken;
            return this.ProcessAccessToken(context, token, verifier);
        }

        /// <summary>
        /// Gets auth headers
        /// </summary>
        /// <param name="token"> Security token. </param>
        /// <param name="requestUri"> Request uri. </param>
        /// <param name="method"> Http method. </param>
        /// <returns> Headers map</returns>
        public Dictionary<string, string> GetAuthHeaders(OAuthSecurityToken token, Uri requestUri, string method)
        {
            Dictionary<string, string> headers = null;

            // Authorization header in case of signed request
            // var oauthToken = token as OAuthSecurityToken;
            if (token != null)
            {
                var oauth = this.GetManager(token);
                oauth["token"] = token.Token;
                oauth["token_secret"] = token.TokenSecret;

                headers = new Dictionary<string, string>
                    {
                        { "Authorization", oauth.GenerateAuthzHeader(requestUri.ToString(), method) } 
                    };
            }

            return headers;
        }

        /// <summary>
        /// Gets auth query string
        /// </summary>
        /// <param name="token"> Security token. </param>
        /// <param name="requestUri"> Request uri. </param>
        /// <param name="method"> Http method. </param>
        /// <returns> Query string</returns>
        public string GetAuthQueryString(OAuthSecurityToken token, Uri requestUri, string method)
        {
            return null;
        }

        /// <summary>
        /// True, if request is oauth 1.0a
        /// </summary>
        /// <param name="context"> Proxy context. </param>
        /// <param name="token"> Security token. </param>
        /// <returns> The can handle. </returns>
        public bool CanHandle(ProxyContext context, OAuthSecurityToken token)
        {
            return token != null ? token.Type == AuthType.OAuth : !string.IsNullOrEmpty(context.Http.Request["oauth_token"]);
        }

        /// <summary>
        /// Extracts valid credentials from external storage
        /// </summary>
        /// <param name="token"> Security token. </param>
        /// <param name="service"> Service name. </param>
        /// <returns> Oauth manager </returns>
        private OAuthManager GetManager(OAuthSecurityToken token, string service = null)
        {
            // TODO refactoring this
            if (service == null)
            {
                service = this.GetService(token).Name;
            }

            var consumer = this._consumerProvider.GetOAuth(token.AppUrl, service);

            var oauth = new OAuthManager();
            oauth["consumer_key"] = consumer.Key;
            oauth["consumer_secret"] = consumer.Secret;
            return oauth;
        }

        /// <summary>
        /// Gets service definition
        /// </summary>
        /// <param name="token"> Security token. </param>
        /// <returns> Service definition. </returns>
        private ServiceDefinition GetService(OAuthSecurityToken token)
        {
            var resolvedUri = FileHelper.ResolvePath(token.AppUrl);
            var uri = new Uri(resolvedUri);
            var gadget = this._gadgetFactory.Get(uri);

            // NOTE: which service should be used?
            return gadget.ModulePreferences.OAuth.Services.First();
        }
    }
}
