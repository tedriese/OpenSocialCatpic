// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserPreference.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents an user preference 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Format
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an user preference 
    /// </summary>
    [DataContract]
    public class UserPreference
    {
        /// <summary>
        /// UserPreference type
        /// </summary>
        public enum DataType
        {
            /// <summary>
            /// A string accessed by gadgets.Prefs.getString. Default data type.
            /// </summary>
            String,

            /// <summary>
            /// a non-visible string that is not user editable but can be accessed by gadgets.Prefs.getString()
            /// </summary>
            Hidden,

            /// <summary>
            /// Evaluates to true or false when accessed by gadgets.Prefs.getBool()
            /// </summary>
            Bool,

            /// <summary>
            /// A pipe-delimited (|) string of values, returned as a JavaScript array when accessed by gadgets.Prefs.getArray().
            /// </summary>
            List,

            /// <summary>
            /// A list of values specified by /UserPref/EnumValue elements. Current value is accessed by gadgets.Prefs.getString()
            /// </summary>
            Enum
        }

        /// <summary>
        /// Gets or sets Name: The name of the preference. Containers MUST provide the current value of the preference
        /// when this key is passed to the getters found in gadgets.Prefs (Section 13.9). This is also the key used 
        /// when performing ${Prefs} variable substitutions (see Variable Substitution (Section 9)).
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Type: The data type for this preference. Valid values are "string", "hidden", "bool", "list", and "number". The default value is "string".
        /// </summary>
        [DataMember(Name = "datatype")]
        public DataType Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not a valid value needs to be set for this preference in order for the gadget to function correctly. 
        /// Valid values are "true" and "false" (default). If the value is "true", Containers SHOULD display an error message or a prompt if there is no valid value stored.
        /// </summary>
        [DataMember(Name = "required")]
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets DisplayName: The name of the preference for use when rendering an editing interface for prefs. Developers SHOULD use this value to provide a localized name
        /// for the preference as described in Localization (Section 10). Containers SHOULD use this value to render any editing interfaces for Users.
        /// </summary>
        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets DefaultValue: A default value for this preference. Containers MUST provide this value for any calls to the getters in gadgets.Prefs (Section 13.9) whenever the key matches @name.
        /// </summary>
        [DataMember(Name = "defaultValue")]
        public object DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets EnumValues.
        /// </summary>
        [DataMember(Name = "enumValues")]
        public IEnumerable<EnumValue> EnumValues { get; set; }
    }
}
