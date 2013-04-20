// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GadgetParser.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Parses gadget's xml to domain-specific representation of gadget. Implementation must be thread-safe
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Format
{
    using System;
    using System.Xml.Linq;

    /// <summary>
    /// Parses gadget's xml to domain-specific representation of gadget. Implementation must be thread-safe
    /// </summary>
    public interface IGadgetParser
    {
        /// <summary>
        /// Parses gadget definition from xml
        /// </summary>
        /// <param name="xdocGadget"> The xdoc gadget. </param>
        /// <param name="baseUri"> The base uri. </param>
        /// <returns> Gadget definition </returns>
        GadgetDefinition Parse(XDocument xdocGadget, Uri baseUri);
    }
}
