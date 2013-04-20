// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFeatureSet.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents feature set
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Containers
{
    using System.Collections.Generic;
    using Catpic.Gadgets.Format;

    /// <summary>
    /// Represents feature set
    /// </summary>
    public interface IFeatureSet
    {
        /// <summary>
        /// Gets or sets name of the feature set
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets all features
        /// </summary>
        IEnumerable<FeatureDefinition> Features { get; }

        /// <summary>
        /// Resolve feature dependencies to plain list
        /// </summary>
        /// <param name="feature">Feature instance</param>
        /// <returns>The lost of feature definitions</returns>
        IEnumerable<FeatureDefinition> ResolveDependencies(FeatureDefinition feature);
    }
}
