// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GadgetDefinitionFactory.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Defines behavior of gadget definition factory
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets
{
    using System;
    using System.Net;

    using Catpic.Gadgets.Format;

    /// <summary>
    /// Defines behavior of gadget definition factory
    /// </summary>
    public interface IGadgetDefinitionFactory
    {
        /// <summary>
        /// Creates gadget from response.
        /// </summary>
        /// <param name="uri">Gadget uri</param>
        /// <param name="response">Gadget response which contains definition</param>
        /// <returns> Gadget definition</returns>
        GadgetDefinition Create(Uri uri, WebResponse response);

        /// <summary>
        /// Returns gadget if it is exists in cache, null otherwise.
        /// </summary>
        /// <param name="uri">Gadget uri</param>
        /// <returns> Gadget definition</returns>
        GadgetDefinition Get(Uri uri);
    }
}