// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OAuth2Definition.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Supplies the Container with OAuth 2.0 service configuration for the gadget
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Format.OAuth2
{
    using System.Collections.Generic;

    /// <summary>
    /// Supplies the Container with OAuth 2.0 service configuration for the gadget
    /// </summary>
    public class OAuth2Definition
    {
        /// <summary>
        /// Gets or sets Services.
        /// </summary>
        public IEnumerable<Service2Definition> Services { get; set; }
    }
}
