namespace Catpic.Gadgets.Format
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    /// <summary>
    /// Parses userPref sections
    /// </summary>
    public class UserPreferencesParser
    {
        /// <summary>
        /// Parses user preferences from xml
        /// </summary>
        /// <param name="xGadgetRoot"> The x gadget root. </param>
        /// <returns> The list of user preferences</returns>
        public virtual IEnumerable<UserPreference> Parse(XElement xGadgetRoot)
        {
            foreach (var xUserPref in xGadgetRoot.Elements("UserPref"))
            {
                var userPref = new UserPreference();
                
                // TODO assert values
                userPref.Name = xUserPref.Attribute("name").Value;

                var required = xUserPref.Attribute("required");
                if (required != null)
                {
                    userPref.IsRequired = bool.Parse(required.Value);
                }

                var xUserPrefAttr = xUserPref.Attribute("datatype");
                userPref.Type = xUserPrefAttr != null ?
                    (UserPreference.DataType)Enum.Parse(typeof(UserPreference.DataType), xUserPref.Attribute("datatype").Value, true) :
                    UserPreference.DataType.String;
                var displayName = xUserPref.Attribute("display_name");
                if (displayName != null)
                {
                    userPref.DisplayName = displayName.Value;
                }

                var defaultValueAttr = xUserPref.Attribute("default_value");
                userPref.DefaultValue = defaultValueAttr != null ? defaultValueAttr.Value : string.Empty;

                // process enum values
                if (userPref.Type == UserPreference.DataType.Enum)
                {
                    IList<EnumValue> enumValues = new List<EnumValue>();
                    foreach (var xEnumValue in xUserPref.Elements("EnumValue"))
                    {
                        // value should set
                        var value = xEnumValue.Attribute("value").Value;

                        // sometimes display isn't set
                        var xDisplayValue = xEnumValue.Attribute("display_value");
                        var enumValue = new EnumValue()
                            { 
                                Value = value, 
                                DisplayValue = xDisplayValue == null ? value : xDisplayValue.Value 
                            };
                        enumValues.Add(enumValue);
                    }

                    userPref.EnumValues = enumValues;
                }

                yield return userPref;
            }
        }
    }
}
