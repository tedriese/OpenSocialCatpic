// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GadgetDefinition.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents gadget
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Format
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents gadget
    /// </summary>
    public class GadgetDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GadgetDefinition"/> class.
        /// </summary>
        public GadgetDefinition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GadgetDefinition"/> class.
        /// </summary>
        /// <param name="modulePrefs"> The module prefs. </param>
        /// <param name="userPrefs"> The user prefs. </param>
        /// <param name="views"> The views. </param>
        public GadgetDefinition(ModulePreferences modulePrefs, IEnumerable<UserPreference> userPrefs, IEnumerable<View> views)
        {
            this.ModulePreferences = modulePrefs;
            this.UserPreferences = userPrefs;
            this.Views = views;
        }

        /// <summary>
        /// Gets or sets ModulePreferences.
        /// </summary>
        public ModulePreferences ModulePreferences { get; set; }

        /// <summary>
        /// Gets or sets UserPreferences.
        /// </summary>
        public IEnumerable<UserPreference> UserPreferences { get; set; }

        /// <summary>
        /// Gets or sets Views.
        /// </summary>
        public IEnumerable<View> Views { get; set; }
    }
}
