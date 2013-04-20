// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureSet.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents feature set of container
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Containers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using Catpic.Gadgets.Format;
    using Catpic.Utils;
    using Catpic.Utils.Configuration;

    /// <summary>
    /// Represents feature set of container
    /// </summary>
    public class FeatureSet : IFeatureSet
    {
        /// <summary>
        /// The list of path to resorce features
        /// </summary>
        private readonly IList<string> _resourcesPaths = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureSet"/> class.
        /// </summary>
        /// <param name="name"> The name of feature set. </param>
        /// <param name="location"> Location of features. </param>
        /// <param name="indexFile"> Name of index file. </param>
        /// <param name="resourcesPaths"> The list of path to resorce features. </param>
        public FeatureSet(string name, string location, string indexFile, IList<string> resourcesPaths)
        {
            this.Name = name;
            this._resourcesPaths = resourcesPaths;
            this.Build(location, indexFile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureSet"/> class.
        /// </summary>
        /// <param name="config"> Configuration of feature set. </param>
        [Obsolete("Supporting of legacy configuration will be removed in future")]
        public FeatureSet(IConfigSection config)
        {
            this.Name = config.GetString("@name");
            var location = config.GetString("@location");
            var index = config.GetString("@index");


            foreach (var resourcePathConfig in config.GetSections("resources/include"))
            {
                var path = resourcePathConfig.GetString("@path");
                this._resourcesPaths.Add(path);
            }

            this.Build(location, index);
        }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets Features.
        /// </summary>
        public IEnumerable<FeatureDefinition> Features { get; private set; }

        /// <summary>
        /// Resolves feature dependencies
        /// </summary>
        /// <param name="feature"> The feature. </param>
        /// <returns> The list of features</returns>
        public IEnumerable<FeatureDefinition> ResolveDependencies(FeatureDefinition feature)
        {
            // TODO catch exceptions in user-friendly way
            return feature.Dependencies.Select(d => this.Features.Single(f => f.Name == d));
        }

        #region private members

        /// <summary>
        /// Builds FeatureSet registry
        /// </summary>
        /// <param name="location">Location of features</param>
        /// <param name="index"> Name of index file</param>
        private void Build(string location, string index)
        {
            var indexPath = Path.Combine(location, index);
            indexPath = FileHelper.ResolvePath(indexPath);

            // get all feature pathes
            var featureRelativePaths = File
                .ReadAllLines(indexPath)
                .Where(line => !line.StartsWith("#") && (!string.IsNullOrEmpty(line)));

            // NOTE "features" part of the path is already defined in index file 
            var directory = Directory.GetParent(indexPath).Parent.FullName;
            this.Features = new List<FeatureDefinition>();
            foreach (var featureRelativePath in featureRelativePaths)
            {
                var featurePath = Path.Combine(directory, featureRelativePath);
                var xdocFeature = XDocument.Load(featurePath);
                (this.Features as IList<FeatureDefinition>).Add(
                    this.GetFeature(
                        xdocFeature,
                        Path.Combine(
                            Path.GetDirectoryName(location.Substring(1)), Path.GetDirectoryName(featureRelativePath))));
            }
        }

        /// <summary>
        /// Returns feature defintion
        /// </summary>
        /// <param name="xdocFeature">XDocument feature representation</param>
        /// <param name="featureLocation">Location of the feature</param>
        /// <returns>Feature definition</returns>
        private FeatureDefinition GetFeature(XDocument xdocFeature, string featureLocation)
        {
            var featureDef = new FeatureDefinition();
            var xFeature = xdocFeature.Root;

            // get name
            string name = xFeature.Element("name").Value;

            // process dependencies
            featureDef.Name = name;
            foreach (var xDependency in xFeature.Elements("dependency"))
            {
                var dependency = xDependency.Value;
                featureDef.Dependencies.Add(dependency);
            }

            /*//process gadget
            var xGadget = xFeature.Element("gadget");
            if (xGadget != null)
            {
                //process script
                foreach (var xScript in xGadget.Elements("script"))
                {
                    var scriptDef = GetScript(xScript, featureLocation);
                    featureDef.Scripts[FeatureTargetType.Gadget].Add(scriptDef);
                }
            }
            //TODO optimize this
            var xAll = xFeature.Element("all");
            if (xAll != null)
            {
                //process script
                foreach (var xScript in xAll.Elements("script"))
                {
                    var scriptDef = GetScript(xScript, featureLocation);
                    featureDef.Scripts[FeatureTargetType.All].Add(scriptDef);
                }
            }
            */
            this.AddScripts(featureDef, xFeature, FeatureTargetType.Gadget, "gadget", featureLocation);
            this.AddScripts(featureDef, xFeature, FeatureTargetType.Container, "container", featureLocation);
            this.AddScripts(featureDef, xFeature, FeatureTargetType.All, "all", featureLocation);

            return featureDef;
        }

        /// <summary>
        /// Adds scripts to feature definition
        /// </summary>
        /// <param name="featureDef"> The feature def. </param>
        /// <param name="xFeature"> The x feature. </param>
        /// <param name="targetType"> The target type. </param>
        /// <param name="xPath"> The x path. </param>
        /// <param name="featureLocation"> The feature location. </param>
        private void AddScripts(FeatureDefinition featureDef, XElement xFeature, FeatureTargetType targetType, string xPath, string featureLocation)
        {
            var xAll = xFeature.Element(xPath);
            if (xAll != null)
            {
                // process script
                foreach (var xScript in xAll.Elements("script"))
                {
                    var scriptDef = this.GetScript(xScript, featureLocation);
                    featureDef.Scripts[targetType].Add(scriptDef);
                }
            }
        }

        /// <summary>
        /// Returns script definition for the script tag
        /// </summary>
        /// <param name="xScript">Xml script representation</param>
        /// <param name="scriptLocation">Script location</param>
        /// <returns>Script definition</returns>
        private ScriptDefinition GetScript(XElement xScript, string scriptLocation)
        {
            var scriptDef = new ScriptDefinition();
            var srcAttr = xScript.Attribute("src");
            
            // it should be inline script
            if (srcAttr == null)
            {
                scriptDef.Type = ScriptContentType.Inline;
                scriptDef.Content = xScript.Value;
                return scriptDef;
            }

            // TODO improve this approach
            if (srcAttr.Value.StartsWith("http://"))
            {
                scriptDef.Type = ScriptContentType.Remote;
                scriptDef.Source = srcAttr.Value;
            }
            else if (srcAttr.Value.StartsWith("res://"))
            {
                scriptDef.Type = ScriptContentType.Resource;
                scriptDef.Source = this.ResolveResource(srcAttr.Value);
            }
            else
            {
                // NOTE assume that it is file which is location in current directory
                scriptDef.Type = ScriptContentType.Local;
                scriptDef.Source = Path.Combine(scriptLocation, srcAttr.Value);
            }

            return scriptDef;
        }

        /// <summary>
        /// Resolves script resource path
        /// </summary>
        /// <param name="src"> The src. </param>
        /// <returns> Path to script</returns>
        private string ResolveResource(string src)
        {
            foreach (var resourcePath in this._resourcesPaths)
            {
                var path = src.Replace("res:/", resourcePath);
                var physicalPath = FileHelper.ResolvePath(path);
                if (File.Exists(physicalPath))
                {
                    return path;
                }
            }

            return null;
        }

        #endregion
    }
}