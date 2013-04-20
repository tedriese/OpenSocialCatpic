// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultContainer.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Default implementation of container which based on shindig client-side API
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Containers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Catpic.Gadgets.Format;
    using Catpic.Utils;
    using Catpic.Utils.Configuration;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Default implementation of container which based on shindig client-side API
    /// </summary>
    public class DefaultContainer : IContainer
    {
        /// <summary>
        /// Feature provider
        /// </summary>
        private readonly IFeatureProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultContainer"/> class.
        /// </summary>
        /// <param name="name"> Name of container. </param>
        /// <param name="containerConfig"> Container config path. </param>
        /// <param name="sets"> Feauture sets. </param>
        /// <param name="factory"> Gadget definition factory. </param>
        public DefaultContainer(
            string name,
            string containerConfig,
            IEnumerable<ContainerFeatureEntry> sets,
            GadgetDefinitionFactory factory)
        {
            this.Name = name;
            this.GadgetFactory = factory;
            this.Features = new List<FeatureDefinition>();
            this.ContainerFeatures = new List<FeatureDefinition>();
            this.CoreFeatures = new List<FeatureDefinition>();

            foreach (var containerFeatureEntry in sets)
            {
                var featureSet = containerFeatureEntry.FeatureSet;
                (this.Features as List<FeatureDefinition>).AddRange(featureSet.Features);
                (this.ContainerFeatures as List<FeatureDefinition>).AddRange(this.GetConfigDefinedFeatures(featureSet, containerFeatureEntry.ContainerFeatures));
                (this.CoreFeatures as List<FeatureDefinition>).AddRange(this.GetConfigDefinedFeatures(featureSet, containerFeatureEntry.GadgetFeatures));
            }

            this.ContainerSettings = this.GetContainerSettings(containerConfig);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultContainer"/> class.
        /// </summary>
        /// <param name="provider"> Feature provider. </param>
        /// <param name="factory"> Gadget definition factory. </param>
        /// <param name="config"> Configuration. </param>
        [Obsolete("Supporting of legacy configuration will be removed in future")]
        public DefaultContainer(IFeatureProvider provider, GadgetDefinitionFactory factory, IConfigSection config)
        {
            this._provider = provider;
            this.GadgetFactory = factory;

            this.Name = config.GetString("@name");
            this.Features = new List<FeatureDefinition>();
            this.ContainerFeatures = new List<FeatureDefinition>();
            this.CoreFeatures = new List<FeatureDefinition>();

            // merge feature sets
            foreach (var featureSetConfig in config.GetSections("features/include"))
            {
                var featureSetName = featureSetConfig.GetString("@set");
                var featureSet = this._provider.GetFeatureSet(featureSetName);
                (this.Features as List<FeatureDefinition>).AddRange(featureSet.Features);
                (this.ContainerFeatures as List<FeatureDefinition>).AddRange(this.GetConfigDefinedFeatures(featureSetConfig.GetSection("container"), featureSet));
                (this.CoreFeatures as List<FeatureDefinition>).AddRange(this.GetConfigDefinedFeatures(featureSetConfig.GetSection("core"), featureSet));
            }

            // initialize client API settings
            var containerSettingsStorage = config.GetString("config/@path");
            this.ContainerSettings = this.GetContainerSettings(containerSettingsStorage);
        }

        #region IContainer members

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets GadgetFactory.
        /// </summary>
        public GadgetDefinitionFactory GadgetFactory { get; private set; }

        /// <summary>
        /// Gets ContainerFeatures.
        /// </summary>
        public IEnumerable<FeatureDefinition> ContainerFeatures { get; private set; }

        /// <summary>
        /// Gets CoreFeatures.
        /// </summary>
        public IEnumerable<FeatureDefinition> CoreFeatures { get; private set; }

        /// <summary>
        /// Gets Features.
        /// </summary>
        public IEnumerable<FeatureDefinition> Features { get; private set; }

        /// <summary>
        /// Gets ContainerSettings.
        /// </summary>
        public string ContainerSettings { get; private set; }

        /// <summary>
        /// Resolves dependencies of feature
        /// </summary>
        /// <param name="feature"> Feature definition. </param>
        /// <returns> The list of dependencies</returns>
        public IEnumerable<FeatureDefinition> ResolveDependencies(FeatureDefinition feature)
        {
            var dependencies = new List<FeatureDefinition>();
            this.TraverseDependencies(feature, dependencies);
            return dependencies.Distinct(new FeatureDefinitionComparer());
        }

        #endregion

        #region private members

        /// <summary>
        /// Traverses dependencies using Depth First Search al.
        /// </summary>
        /// <param name="feature"> Target feature. </param>
        /// <param name="features"> Features tree. </param>
        private void TraverseDependencies(FeatureDefinition feature, List<FeatureDefinition> features)
        {
            foreach (var featureName in feature.Dependencies)
            {
                var fd = this.GetFeatureDefinition(featureName);
                this.TraverseDependencies(fd, features);
            }

            // prevent appending of collection of already existing item
            // if (!features.Contains(feature, new FeatureDefinitionComparer()))
            features.Add(feature);
        }

        /// <summary>
        /// Gets feature definition by name.
        /// </summary>
        /// <param name="name"> Feature name. </param>
        /// <returns> Feature definition</returns>
        private FeatureDefinition GetFeatureDefinition(string name)
        {
            return this.Features.Single(f => f.Name == name);
        }

        /// <summary>
        /// Returns shindig's container settings in json
        /// </summary>
        /// <param name="path"> Path to shindig's json configuration file. </param>
        /// <returns> Returns environment specific format string </returns>
        private string GetContainerSettings(string path)
        {
            var content = FileHelper.GetContent(path);
            var normalizedJson = JsonHelper.Uncomment(content);
            var jContainer = JsonConvert.DeserializeObject<JObject>(normalizedJson);
            foreach (JProperty child in jContainer.Children())
            {
                var closure = child;
                if (closure.Name == "gadgets.features")
                {
                    return child.Value.ToString();
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns the list of features defined in configuration
        /// </summary>
        /// <param name="featureSetConfig"> Configuration of feature set. </param>
        /// <param name="featureSet"> Feature set. </param>
        /// <returns> The list of features </returns>
        [Obsolete("Supporting of legacy configuration will be removed in future")]
        private IEnumerable<FeatureDefinition> GetConfigDefinedFeatures(IConfigSection featureSetConfig, IFeatureSet featureSet)
        {
            var features = new List<FeatureDefinition>();
            foreach (var coreFeatureConfig in featureSetConfig.GetSections("feature"))
            {
                var coreFeactureName = coreFeatureConfig.GetString("@name");
                var coreFeature = featureSet.Features.Single(f => f.Name == coreFeactureName);
                this.TraverseDependencies(coreFeature, features);
            }

            // distinc features list
            return features.Distinct(new FeatureDefinitionComparer());
        }

        /// <summary>
        /// Returns the list of features defined in configuration
        /// </summary>
        /// <param name="featureSet"> Feature set. </param>
        /// <param name="names"> Names of features. </param>
        /// <returns> The list of features </returns>
        private IEnumerable<FeatureDefinition> GetConfigDefinedFeatures(FeatureSet featureSet, IEnumerable<string> names)
        {
            var features = new List<FeatureDefinition>();
            foreach (var name in names)
            {
                var coreFeature = featureSet.Features.Single(f => f.Name == name);
                this.TraverseDependencies(coreFeature, features);
            }

            // distinc features list
            return features.Distinct(new FeatureDefinitionComparer());
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents container feature entry
        /// </summary>
        public class ContainerFeatureEntry
        {
            /// <summary>
            /// Gets or sets FeatureSet.
            /// </summary>
            public FeatureSet FeatureSet { get; set; }

            /// <summary>
            /// Gets or sets ContainerFeatures.
            /// </summary>
            public IEnumerable<string> ContainerFeatures { get; set; }

            /// <summary>
            /// Gets or sets GadgetFeatures.
            /// </summary>
            public IEnumerable<string> GadgetFeatures { get; set; }
        }
        #endregion
    }
}