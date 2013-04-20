using System;
using System.Collections.Generic;

namespace Catpic.Host.Engine.Navigation
{
    /// <summary>
    /// Represents navigation tree node
    /// </summary>
    [Serializable]
    public class NavigationNode : INavigationNode
    {
        public NavigationNode(Inventory inventory, INavigationEntry entry)
        {
            Inventory = inventory;
            Entry = entry;
            IsVisible = true;
        }

        #region Implementation of INavigationNode

        
        public INavigationEntry Entry { get; private set; }
        public Inventory Inventory { get; private set; }
        public IDictionary<string, string> Properties { get; set; }

        /// <summary>
        /// Should be hidden
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// True if it represents current page
        /// </summary>
        public bool IsCurrent { get;  set; }

        /// <summary>
        /// Child nodes
        /// </summary>
        public IEnumerable<INavigationNode> Children { get; set; }

        #endregion
    }
}