// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentParser.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Parses content node
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Format
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Parses content node
    /// </summary>
    public class ContentParser
    {
        /// <summary>
        /// Parse content section
        /// </summary>
        /// <param name="xGadgetRoot"> The x gadget root. </param>
        /// <returns> The list of views</returns>
        public IEnumerable<View> Parse(XElement xGadgetRoot)
        {
            var views = new List<View>();
            foreach (var xContent in xGadgetRoot.Elements("Content"))
            {
                View.ViewType viewType = View.ViewType.Html;
                var xType = xContent.Attribute("type");
                if (xType != null)
                {
                    viewType = (View.ViewType)Enum.Parse(typeof(View.ViewType), xContent.Attribute("type").Value, true);
                }

                Uri uri = null;
                if (viewType == View.ViewType.Url)
                {
                    uri = new Uri(xContent.Attribute("href").Value);
                }

                var prefHeightAttr = xContent.Attribute("preferred_height");
                int prefHeight = prefHeightAttr == null ? 0 : int.Parse(prefHeightAttr.Value);

                var prefWidthAttr = xContent.Attribute("preferred_width");
                int prefWidth = prefWidthAttr == null ? 0 : int.Parse(prefWidthAttr.Value);


                var viewNamesAttr = xContent.Attribute("view");

                var viewNames = viewNamesAttr != null
                                    ? viewNamesAttr.Value.Split(',').Select(s => s.Trim())
                                    : new[] { "default" };
                foreach (var viewName in viewNames)
                {
                    // NOTE all view have the same reference to xml content
                    views.Add(new View(xContent.Value)
                                  {
                                      Name = viewName,
                                      Type = viewType,
                                      Href = uri,
                                      PreferredHeight = prefHeight,
                                      PreferredWidth = prefWidth
                                  });
                }
            }

            return views;
        }
    }
}
