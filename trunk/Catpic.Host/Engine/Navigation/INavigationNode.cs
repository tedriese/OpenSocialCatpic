using System;
using System.Collections.Generic;

namespace Catpic.Host.Engine.Navigation
{
    /// <summary>
    /// Represents navigation entry
    /// </summary>
    public interface INavigationNode
    {
        /// <summary>
        /// Entry which stores label and properties
        /// </summary>
        INavigationEntry Entry { get; }

        /// <summary>
        /// Usually a path to view
        /// </summary>
        Inventory Inventory { get; }

        /// <summary>
        /// Properties mapping
        /// </summary>
        IDictionary<string, string> Properties { get; set; }

        /// <summary>
        /// Should be hidden TODO create property map instead
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// True, if entry is current
        /// </summary>
        bool IsCurrent { get; set; }

        /// <summary>
        /// Returns children of node
        /// </summary>
        IEnumerable<INavigationNode> Children { get; }
    }
}