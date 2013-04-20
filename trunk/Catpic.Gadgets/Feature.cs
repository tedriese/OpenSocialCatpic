// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Feature.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Feature entity of gadget module description
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Feature entity of gadget module description
    /// </summary>
    public class Feature
    {
        /// <summary>
        /// Gets or sets name of the name of feature.
        /// </summary>
        [DataMember(Name = "feature")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Version. The version of the Feature to provide. Containers MUST interpret this value as described in Versioning (Section 8). If absent, the default value is "1.0".       
        /// </summary>
         [DataMember(Name = "version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets Views. Spec: a comma-separated list of Views to load this feature for. This parameter is optional. Containers SHOULD only load the feature when an appropriate view is being rendered.
        /// </summary>
        [DataMember(Name = "views")]
        public IEnumerable<View> Views { get; set; }

        /// <summary>
        /// Gets or sets specific feature parameters.
        /// </summary>
        public IDictionary<string, string> Parameteres { get; set; }
    }
}
