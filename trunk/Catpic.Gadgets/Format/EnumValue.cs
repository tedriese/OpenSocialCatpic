// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumValue.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   A single value that serves as a constraint on User Preferences when /UserPref/@datatype is "enum"
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Format
{
    using System.Runtime.Serialization;

    /// <summary>
    /// A single value that serves as a constraint on User Preferences when /UserPref/@datatype is "enum"
    /// </summary>
    [DataContract]
    public class EnumValue
    {
        /// <summary>
        /// Gets or sets Value for this enumeration element
        /// </summary>
        [DataMember(Name = "value")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets DisplayValue: a textual representation of @value. Defaults to the value of @value.
        /// Containers SHOULD display this value in place of @value when rendering a user interface for editing preferences.
        /// </summary>
        [DataMember(Name = "display_value")]
        public string DisplayValue { get; set; }
    }
}
