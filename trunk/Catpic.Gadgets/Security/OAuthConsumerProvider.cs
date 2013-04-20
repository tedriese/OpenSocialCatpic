// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OAuthConsumerProvider.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Default consumer provider
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Security
{
    using System.Collections.Generic;
    using System.Linq;

    using Catpic.Gadgets.Security.OAuth;
    using Catpic.Gadgets.Security.OAuth2;

    /// <summary>
    /// Default consumer provider
    /// </summary>
    public class OAuthConsumerProvider : IOAuthConsumerProvider
    {
        /// <summary>
        /// Oauth1.0 consumers
        /// </summary>
        private readonly IEnumerable<OAuthConsumer> _oauthConsumers;

        /// <summary>
        /// Oauth2 consumers
        /// </summary>
        private readonly IEnumerable<OAuth2Consumer> _oauth2Consumers;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthConsumerProvider"/> class.
        /// </summary>
        /// <param name="oauthConsumers"> Oauth consumers. </param>
        /// <param name="oauth2Consumers"> Oauth 2 consumers. </param>
        public OAuthConsumerProvider(IEnumerable<OAuthConsumer> oauthConsumers, IEnumerable<OAuth2Consumer> oauth2Consumers)
        {
            this._oauthConsumers = oauthConsumers;
            this._oauth2Consumers = oauth2Consumers;
        }

        /// <summary>
        /// Gets oauth1 consumer
        /// </summary>
        /// <param name="appId"> Application id. </param>
        /// <param name="service"> Oauth1 service name. </param>
        /// <returns> Oauth1 consumer</returns>
        public OAuthConsumer GetOAuth(string appId, string service)
        {
            return this._oauthConsumers.SingleOrDefault(c => c.AppId == appId && c.Service == service);
        }

        /// <summary>
        /// Gets oauth2 consumer
        /// </summary>
        /// <param name="appId"> Application id. </param>
        /// <param name="service"> Oauth2 service name. </param>
        /// <returns> Oauth2 consumer</returns>
        public OAuth2Consumer GetOAuth2(string appId, string service)
        {
            return this._oauth2Consumers.SingleOrDefault(c => c.AppId == appId && c.Service == service);
        }
    }
}
