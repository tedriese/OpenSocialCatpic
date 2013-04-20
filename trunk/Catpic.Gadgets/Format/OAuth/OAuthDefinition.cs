// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OAuthDefinition.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Supplies the Container with OAuth service configuration for the gadget. For details, see OAuth (Section 11)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Format.OAuth
{
    using System.Collections.Generic;

    /// <summary>
    /// Supplies the Container with OAuth service configuration for the gadget. For details, see OAuth (Section 11)
    /// </summary>
    public class OAuthDefinition
    {
        /// <summary>
        /// Gets or sets Services.
        /// </summary>
        public IEnumerable<ServiceDefinition> Services { get; set; }
    }
}
