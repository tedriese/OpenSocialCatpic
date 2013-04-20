// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureDefinition.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents feature
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Format
{
    using System.Collections.Generic;

    /// <summary>
    /// Type of feature target object
    /// </summary>
    public enum FeatureTargetType
    {
        /// <summary>
        /// Gadget feature.
        /// </summary>
        Gadget,

        /// <summary>
        /// Container feature.
        /// </summary>
        Container,

        /// <summary>
        /// Container and gadgets feature.
        /// </summary>
        All
    }

    /// <summary>
    /// Feature definition extensions
    /// </summary>
    public static class FeatureExtensions
    {
        /// <summary>
        /// Gets container scripts
        /// </summary>
        /// <param name="feature"> The feature. </param>
        /// <returns> the list of feature definitions.</returns>
        public static IEnumerable<ScriptDefinition> GetContainerScripts(this FeatureDefinition feature)
        {
            var scripts = new List<ScriptDefinition>();
            scripts.AddRange(feature.Scripts[FeatureTargetType.Container]);
            scripts.AddRange(feature.Scripts[FeatureTargetType.All]);
            return scripts;
        }

        /// <summary>
        /// Gets Gadget scripts
        /// </summary>
        /// <param name="feature"> The feature. </param>
        /// <returns> the list of feature definitions.</returns>
        public static IEnumerable<ScriptDefinition> GetGadgetScripts(this FeatureDefinition feature)
        {
            var scripts = new List<ScriptDefinition>();
            scripts.AddRange(feature.Scripts[FeatureTargetType.Gadget]);
            scripts.AddRange(feature.Scripts[FeatureTargetType.All]);
            return scripts;
        }
    }

    /// <summary>
    /// Represents feature definition
    /// </summary>
    public class FeatureDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureDefinition"/> class.
        /// </summary>
        public FeatureDefinition()
        {
            this.Dependencies = new List<string>();
            this.Scripts = new Dictionary<FeatureTargetType, IList<ScriptDefinition>>
                {
                    { FeatureTargetType.Gadget, new List<ScriptDefinition>() },
                    { FeatureTargetType.Container, new List<ScriptDefinition>() },
                    { FeatureTargetType.All, new List<ScriptDefinition>() }
                };
        }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets TargetType.
        /// </summary>
        public FeatureTargetType TargetType { get; set; }

        /// <summary>
        /// Gets or sets Dependencies.
        /// </summary>
        public IList<string> Dependencies { get; set; }

        /// <summary>
        /// Gets or sets Scripts.
        /// </summary>
        public IDictionary<FeatureTargetType, IList<ScriptDefinition>> Scripts { get; set; }
    }

    /// <summary>
    /// Compares feature definition by name
    /// </summary>
    public class FeatureDefinitionComparer : IEqualityComparer<FeatureDefinition>
    {
        /// <summary>
        /// Compares two features
        /// </summary>
        /// <param name="x"> First feature. </param>
        /// <param name="y"> Second Feature. </param>
        /// <returns> True, if features are equal. </returns>
        public bool Equals(FeatureDefinition x, FeatureDefinition y)
        {
            return x.Name == y.Name && x.TargetType == y.TargetType;
        }

        /// <summary>
        /// Calcs hash code of feature
        /// </summary>
        /// <param name="obj"> Feature instance. </param>
        /// <returns> Hash code </returns>
        public int GetHashCode(FeatureDefinition obj)
        {
            return obj.GetHashCode();
        }
    }
}
