using System;
using System.Collections.Generic;

namespace Catpic.Host.Engine.Navigation
{
    /// <summary>
    /// Basic implementation of NavigationEntity
    /// </summary>
    [Serializable]
    public class NavigationEntry : INavigationEntry
    {
        public NavigationEntry(string id, string label)
        {
            Id = id;
            Label = label;
        }

        #region INavigationEntry implementation

        public string Id { get; set; }
        public string Label { get; set; }
        public IDictionary<string, string> Properties { get; set; }

        #endregion
    }
}