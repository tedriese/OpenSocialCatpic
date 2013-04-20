// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceDefinition.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   A single OAuth service configuration
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Format.OAuth
{
    using System;

    /// <summary>
    /// A single OAuth service configuration
    /// </summary>
    public class ServiceDefinition
    {
        /// <summary>
        /// Gets or sets the name of the service, used for referencing OAuth services at runtime. This parameter is optional, and if unspecified defaults to an empty string. Gadget developers specify which OAuth service they wish to use by passing the service name as a parameter to gadgets.io.makeRequest (Section 12.2.1.3).
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        ///  Gets or sets the OAuth request token URL. See OAuth (Section 11)for details.
        /// </summary>
        public TokenRequest Request { get; set; }
        
        /// <summary>
        ///  Gets or sets the OAuth access token URLs. See OAuth (Section 11)for details.
        /// </summary>
        public TokenRequest Access { get; set; }

        /// <summary>
        ///  Gets or sets the OAuth authorization URL. For details, see Section 6.2 of OAuth specification [OAuth-Core].
        /// </summary>
        public Uri Authorization { get; set; }
    }
}
