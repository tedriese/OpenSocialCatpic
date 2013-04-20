using System;
using System.Collections.Generic;

namespace Catpic.Host.Engine.Navigation
{
    /// <summary>
    /// Represents navigarion entry
    /// </summary>
    public interface INavigationEntry
    {
        /// <summary>
        /// Id of navigation entry
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Label 
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Properties
        /// </summary>
        IDictionary<string, string> Properties { get; }
    }
}