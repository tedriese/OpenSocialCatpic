// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OAuth2Consumer.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Defines the OAuth2Consumer type which represents oauth2 service consumer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Security.OAuth2
{
    /// <summary>
    /// Represents oauth2 service consumer
    /// </summary>
    public class OAuth2Consumer
    {
        /// <summary>
        /// Gets or sets application id
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Gets or sets  service name
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// Gets or sets client id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets secret
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Gets or sets type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether auth headers are used for token delivery.
        /// </summary>
        public bool UsesAuthorizationHeader { get; set; }
    }
}
