// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOAuthConsumerProvider.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Defines consumer provider behavior
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Security
{
    using Catpic.Gadgets.Security.OAuth;
    using Catpic.Gadgets.Security.OAuth2;

    /// <summary>
    /// Defines consumer provider behavior
    /// </summary>
    public interface IOAuthConsumerProvider
    {
        /// <summary>
        /// Returns oauth1.0a consumer by appid and service name
        /// </summary>
        /// <param name="appId"> Application id. </param>
        /// <param name="service"> Oauth service. </param>
        /// <returns> Oauth consumer. </returns>
        OAuthConsumer GetOAuth(string appId, string service);

        /// <summary>
        /// Returns oauth2 consumer by appid and service name
        /// </summary>
        /// <param name="appId"> Application id. </param>
        /// <param name="service"> Oauth service. </param>
        /// <returns> Oauth consumer. </returns>
        OAuth2Consumer GetOAuth2(string appId, string service);
    }
}