// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContainer.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents specific environment for executing of gadgets
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Containers
{
    using System.Collections.Generic;

    using Catpic.Gadgets.Format;

    /// <summary>
    /// Represents specific environment for executing of gadgets
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets all container features
        /// </summary>
        IEnumerable<FeatureDefinition> Features { get; }

        /// <summary>
        /// Gets container features
        /// </summary>
        IEnumerable<FeatureDefinition> ContainerFeatures { get; }

        /// <summary>
        /// Gets core features
        /// </summary>
        IEnumerable<FeatureDefinition> CoreFeatures { get; }

        /// <summary>
        /// Gets gadget definition factory
        /// </summary>
        GadgetDefinitionFactory GadgetFactory { get; }

        /// <summary>
        /// Gets features settings in JSON
        /// </summary>
        /// <returns>Enviroment-specific string</returns>
        string ContainerSettings { get; }

        /// <summary>
        /// Resolves dependencies of feature into set.
        /// </summary>
        /// <param name="feature">Feature to resolve.</param>
        /// <returns>The list of feature definitions.</returns>
        IEnumerable<FeatureDefinition> ResolveDependencies(FeatureDefinition feature);
    }
}
