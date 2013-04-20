// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocaleDefinition.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents locale node of gadget definition
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Format
{
    /// <summary>
    /// Represents locale node of gadget definition
    /// </summary>
    public class LocaleDefinition
    {
        /// <summary>
        /// Gets or sets Language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets Direction.
        /// </summary>
        public LanguageDirection Direction { get; set; }

        /// <summary>
        /// Gets or sets Messages.
        /// </summary>
        public MessageBundle Messages { get; set; }
    }
}
