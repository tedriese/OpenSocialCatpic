using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Catpic.Host.Engine.Navigation
{
    /// <summary>
    /// Navigation provider which reads menu from xml file
    /// </summary>
    public class XmlNavigationProvider : INavigationProvider
    {
        private readonly INavigationNode _root;

        public XmlNavigationProvider(Stream stream)
        {
            var node = GetConfigDocument(stream);
            _root = ToNavigationNode(node);
        }
 
        #region static members

        /// <summary>
        /// Reads file from the path and return XElement for the target navigation node
        /// </summary>
        private static XElement GetConfigDocument(Stream stream)
        {
            var document = XDocument.Load(stream);
            return document.Root;
        }

        /// <summary>
        /// Convert XElement to navigation node
        /// </summary>
        private static INavigationNode ToNavigationNode(XElement element)
        {
            return Traverse(element);
        }

        /// <summary>
        /// Build menu tree from XElement recursively
        /// </summary>
        private static INavigationNode Traverse(XElement element)
        {
            NavigationEntry entry = new NavigationEntry(
                GetAttributeValue("id", element),
                GetAttributeValue("label", element));

            //create inventory
            var inventoryId = GetAttributeValue("inventory", element);
            Inventory inventory = null;
            if (!String.IsNullOrEmpty(inventoryId))
                inventory = ToInventory(inventoryId);

            NavigationNode node = new NavigationNode(inventory, entry);

            bool isVisible = true;

            if (Boolean.TryParse(GetAttributeValue("visible", element), out isVisible))
                node.IsVisible = isVisible;
            node.Properties = GetAttributesMap(element);

            List<INavigationNode> children = new List<INavigationNode>();
            var xChildren = element.Element("children");
            if (xChildren != null)
            {
                foreach (var xElement in xChildren.Elements("node"))
                {
                    children.Add(Traverse(xElement));
                }
            }

            node.Children = children;

            return node;
        }

        /// <summary>
        /// Return attribute or null
        /// </summary>
        private static string GetAttributeValue(string name, XElement element)
        {
            var attr = element.Attribute(name);
            return attr != null ? attr.Value : null;
        }

        private static IDictionary<string, string> GetAttributesMap(XElement element)
        {
            Dictionary<string,string> map = new Dictionary<string, string>();
            var attrs = element.Attributes();
            foreach (var xAttribute in attrs)
            {
                map.Add(xAttribute.Name.ToString(), xAttribute.Value);
            }
            return map;
        }

        /// <summary>
        /// Get inventory by id
        /// </summary>
        private static Inventory ToInventory(string id)
        {
            //NOTE: just extract inventory fields from id
            string[] parts = id.Split('/');
            string area = parts[0];
            string controller = parts[1];
            string action = "Index";
            if (parts.Length > 2)
                action = parts[2];
            return new Inventory(id, area, controller, action);
        }

        #endregion

        #region Implementation of INavigationProvider

        /// <summary>
        /// Returns menu root
        /// </summary>
        /// <returns></returns>
        public INavigationNode GetRoot()
        {
            return _root;
        }

        #endregion
    }
}