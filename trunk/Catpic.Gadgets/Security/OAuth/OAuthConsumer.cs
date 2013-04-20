// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OAuthConsumer.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents oauth consumer
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Security.OAuth
{
    /// <summary>
    /// Represents oauth consumer
    /// </summary>
    public class OAuthConsumer
    {
        /// <summary>
        /// Gets or sets application id
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Gets or sets service name
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// Gets or sets key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets secret
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Gets or sets key type
        /// </summary>
        public string KeyType { get; set; }
    }
}
