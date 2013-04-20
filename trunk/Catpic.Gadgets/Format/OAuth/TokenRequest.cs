// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TokenRequest.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents the OAuth token URL
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Format.OAuth
{
    using System;

    /// <summary>
    /// Represents the OAuth token URL
    /// </summary>
    public class TokenRequest
    {
        /// <summary>
        ///  Gets or sets the URL for the endpoint.
        /// </summary>
        public Uri Endpoint { get; set; }

        /// <summary>
        ///  Gets or sets the HTTP verb to use for making the request. Containers MUST use this HTTP method when sending the request to the service URL. This parameter is optional. If unspecified, it defaults to POST.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets one of 3 possible locations in the request to the service where the OAuth parameters may be passed. Developers MAY use this value to specify the
        /// location of OAuth-related parameters. The possible values are "uri-query", "auth-header", and "post-body", corresponding to the options described 
        /// in Section 5.2 of the OAuth specification [OAuth-Core]. The default value is "auth-header".
        /// </summary>
        public string ParamLocation { get; set; }
    }
}
