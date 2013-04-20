// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreloadDefinition.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represent an HTTP request to pre-fetch
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Format
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represent an HTTP request to pre-fetch
    /// </summary>
    public class PreloadDefinition
    {
        /// <summary>
        /// Gets or sets the URL of the request to prefetch.
        /// </summary>
        [DataMember(Name = "href")]
        public Uri Href { get; set; }

        /// <summary>
        /// Gets or sets the authentication type to use for making this request. Valid values are the same as for 
        /// gadgets.io.AuthorizationType (Section 13.3), though the names are converted to lower case to appear more natural in XML.
        /// </summary>
        [DataMember(Name = "authz")]
        public string Authz { get; set; }

        /// <summary>
        /// Gets or sets SignViewer which indicates that the Owner should be passed to endpoints when using an authentication type that requires it. 
        /// Containers MUST pass the owner id when this value is "true" (default).
        /// </summary>
        [DataMember(Name = "sign_owner")]
        public string SignOwner { get; set; }

        /// <summary>
        /// Gets or sets SignViewer which Indicates that the Viewer should be passed to endpoints when using an authentication type that requires it. 
        /// Containers MUST pass the viewer id when this value is "true" (default).
        /// </summary>
        [DataMember(Name = "sign_viewer")]
        public string SignViewer { get; set; }

        /// <summary>
        /// Gets or sets Views which represents a comma-separated list of Views to pre-fetch this request for. This parameter is optional.
        /// Containers SHOULD only perform preloading when an appropriate view is being rendered.
        /// </summary>
        [DataMember(Name = "views")]
        public IEnumerable<View> Views { get; set; }
    }
}
