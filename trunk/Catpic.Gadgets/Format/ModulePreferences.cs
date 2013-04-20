// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModulePreferences.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents modulePref section of gadget definition
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Format
{
    using System;
    using System.Collections.Generic;

    using Catpic.Gadgets.Format.OAuth;
    using Catpic.Gadgets.Format.OAuth2;

    /// <summary>
    /// Represents modulePref section of gadget definition
    /// </summary>
    public class ModulePreferences
    {
        /// <summary>
        /// Gets or sets Header.
        /// </summary>
        public IDictionary<string, string> Header { get; set; }

        /// <summary>
        /// Gets or sets Icon.
        /// </summary>
        public IconDefinition Icon { get; set; }

        /// <summary>
        /// Gets or sets RequiredFeatures.
        /// </summary>
        public IEnumerable<Feature> RequiredFeatures { get; set; }

        /// <summary>
        /// Gets or sets OptionalFeatures.
        /// </summary>
        public IEnumerable<Feature> OptionalFeatures { get; set; }

        /// <summary>
        /// Gets or sets Locales.
        /// </summary>
        public IEnumerable<LocaleDefinition> Locales { get; set; }

        /// <summary>
        /// Gets or sets Preloads.
        /// </summary>
        public IEnumerable<PreloadDefinition> Preloads { get; set; }

        /// <summary>
        /// Gets or sets OAuth.
        /// </summary>
        public OAuthDefinition OAuth { get; set; }

        /// <summary>
        /// Gets or sets OAuth2.
        /// </summary>
        public OAuth2Definition OAuth2 { get; set; }
    }
}
