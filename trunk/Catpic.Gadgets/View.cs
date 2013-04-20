// --------------------------------------------------------------------------------------------------------------------
// <copyright file="View.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents content section of gadget definition
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents content section of gadget definition
    /// </summary>
    public class View
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="View"/> class.
        /// </summary>
        /// <param name="content"> Content of view. </param>
        public View(string content)
        {
            this.Content = content;
        }

        #region Nested classes

        /// <summary>
        /// View types
        /// </summary>
        public enum ViewType
        {
            /// <summary>
            /// View defined in gadget definition
            /// </summary>
            Html,

            /// <summary>
            /// View located on specific url
            /// </summary>
            Url
        }

        #endregion 

        /// <summary>
        /// Gets or sets Content. Contains data in an appropriate format to satisfy the requirements for @type. When @type is "html", if no @href is specified, this value MUST be a block of html. 
        /// The text within this element SHOULD be wrapped in CDATA to avoid having to escape HTML tags and to prevent them from being interpreted by the XML parser.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Type. REQUIRED. The type of content included in the body of this element. Valid values are "html" and "url". Containers MUST interpret the body of this element according to the specific processing rules for each type.
        /// When /Content/@type is "html", Containers MUST perform Variable Substitution (Section 9) on the final assembled content section.
        /// When /Content/@type is "url", Containers MUST display a direct view of the value for /Content/@href as specified in the Content Redirect (Section 7) section.
        /// </summary>
        [DataMember(Name = "type")]
        public ViewType Type { get; set; }

        /// <summary>
        /// Gets or sets Href. A URL pointing to an external file containing the body of this element.
        /// If @type is "url", this value is REQUIRED. If @type is "url", this value represents an external document that SHOULD be presented directly to Users without filtering as specified in the Content Redirect (Section 7) section.
        /// If @type is "html", the Container MUST proxy (Section 6) the current content section. Discussion [Issue-1173]
        /// </summary>
        [DataMember(Name = "href")]
        public Uri Href { get; set; }

        /// <summary>
        /// Gets or sets PreferredHeight. The suggested default height, in pixels, to use when rendering this gadget. Containers SHOULD use this value as the default height for a gadget in any environment that allows gadgets to have variable heights. 
        /// If both the @height and @preferred_height are specified, the container SHOULD respect the value of @preferred_height.
        /// </summary>
        [DataMember(Name = "preferred_height")]
        public int PreferredHeight { get; set; }

        /// <summary>
        /// Gets or sets PreferredWidth. The suggested default width, in pixels, to use when rendering this gadget. Containers SHOULD use this value as the default width for a gadget in any environment
        ///  that allows gadgets to have variable widths. If both the @width and @preferred_width are specified, the container SHOULD respect the value of @preferred_width.
        /// </summary>
        [DataMember(Name = "preferred_height")]
        public int PreferredWidth { get; set; }
    }
}
