// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IconDefinition.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Format
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Module icon: a link or embedded image that serves as an icon for the gadget.
    /// </summary>
    [DataContract]
    public class IconDefinition
    {
        /// <summary>
        /// Gets or sets Mode: The encoding used for the icon when embedding an image in the Icon element's child text node. 
        /// Currently, only 'base64' is the only supported value. Containers SHOULD treat child text nodes as a base64 encoded image
        /// when this attribute is provided. Otherwise, child text nodes should be interpreted as an image URL.
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// Gets or sets MimeType: The MIME type of the embedded icon text. Containers SHOULD interpret the encoded child text node using this MIME type.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets value: Developers MUST provide a value for the child text node appropriate to the @mode value.
        /// If @mode is "base64", the child text node MUST be a base64-encoded image in the format specified by @type. 
        /// If @mode is missing, the child text node MUST be a valid URL pointing to an image file.
        /// </summary>
        public string Value { get; set; }
    }
}
