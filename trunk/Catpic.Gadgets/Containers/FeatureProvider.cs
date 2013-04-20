// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureProvider.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Provides feature sets.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Containers
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides feature sets.
    /// </summary>
    public class FeatureProvider : IFeatureProvider
    {
        /// <summary>
        /// The list of feature sets.
        /// </summary>
        private readonly IEnumerable<IFeatureSet> _featureSets;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureProvider"/> class.
        /// </summary>
        /// <param name="featureSets">
        /// The feature sets.
        /// </param>
        public FeatureProvider(IEnumerable<IFeatureSet> featureSets)
        {
            this._featureSets = featureSets;
        }

        #region IFeatureProvider members

        /// <summary>
        /// Gets FeatureSets.
        /// </summary>
        public IEnumerable<IFeatureSet> FeatureSets
        {
            get { return this._featureSets; }
        }

        /// <summary>
        /// Gets feature set by name
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <returns> Feature set</returns>
        public IFeatureSet GetFeatureSet(string name)
        {
            return this._featureSets.Single(f => f.Name == name);
        }

        #endregion
    }
}
