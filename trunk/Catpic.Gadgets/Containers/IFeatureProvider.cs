// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFeatureProvider.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents behavior of feature set provider
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Containers
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents behavior of feature set provider
    /// </summary>
    public interface IFeatureProvider
    {
        /// <summary>
        /// Gets all features
        /// </summary>
        IEnumerable<IFeatureSet> FeatureSets { get; }

        /// <summary>
        /// Gets feature set by its name
        /// </summary>
        /// <param name="name">Name of feature set</param>
        /// <returns>Feature set instance</returns>
        IFeatureSet GetFeatureSet(string name);
    }
}
