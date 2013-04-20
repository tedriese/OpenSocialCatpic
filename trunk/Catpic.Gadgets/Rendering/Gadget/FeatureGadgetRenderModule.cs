// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureGadgetRenderModule.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Renders features
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Rendering.Gadget
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Catpic.Gadgets.Containers;
    using Catpic.Gadgets.Format;
    using Catpic.Utils;
    using Catpic.Utils.Diagnostic;

    using HtmlAgilityPack;

    /// <summary>
    /// Renders features
    /// </summary>
    public class FeatureGadgetRenderModule : IGadgetRenderModule
    {
        /// <summary>
        /// Trace category
        /// </summary>
        private const string TraceCategory = "render.module.feature";

        /// <summary>
        /// Trace instance
        /// </summary>
        private static readonly ITrace Trace = TraceFactory.GetTrace();

        /// <summary>
        /// Renders gadget content into html document
        /// </summary>
        /// <param name="container">Container instance.</param>
        /// <param name="gadget">Gadget instance.</param>
        /// <param name="document">Target html document</param>
        /// <returns>Async task</returns>
        public Task RenderAsync(IContainer container, Catpic.Gadgets.Gadget gadget, HtmlDocument document)
        {
            Trace.Debug(TraceCategory, string.Format("begin {0}", gadget.Context.Uri));
            try
            {
                this.Render(container, gadget, document);
                Trace.Debug(TraceCategory, string.Format("end {0}", gadget.Context.Uri));
            }
            catch (Exception ex)
            {
                Trace.Error(TraceCategory, string.Format("gadget:{0}", gadget.Context.Uri), ex);
            }

            return AsyncHelper.GetEmptyTask();
        }

        /// <summary>
        /// Renders features
        /// </summary>
        /// <param name="container"> Container instance. </param>
        /// <param name="gadget"> Gadget instance. </param>
        /// <param name="document"> Target html document. </param>
        private void Render(IContainer container, Catpic.Gadgets.Gadget gadget, HtmlDocument document)
        {
            var features = new List<FeatureDefinition>();

            // TODO optimize this approach
            features.AddRange(container.CoreFeatures);
            this.FillFeaturesList(container, gadget.Definition.ModulePreferences.RequiredFeatures, features);
            this.FillFeaturesList(container, gadget.Definition.ModulePreferences.OptionalFeatures, features);
            features = features.Distinct(new FeatureDefinitionComparer()).ToList();
            var head = gadget.Context.RenderMode == RenderModeType.Iframe
                           ? document.DocumentNode.SelectSingleNode("html/head")
                           : document.DocumentNode.SelectSingleNode("div");
            foreach (var featureDefinition in features)
            {
                var closure = featureDefinition;
                this.AddFeature(document, head, closure);
            }
        }

        /// <summary>
        /// Fills feature list
        /// </summary>
        /// <param name="container"> Container instance. </param>
        /// <param name="features"> Feature list. </param>
        /// <param name="result"> Result feature list. </param>
        private void FillFeaturesList(IContainer container, IEnumerable<Feature> features, List<FeatureDefinition> result)
        {
            foreach (var feature in features)
            {
                var featureDefinition = this.GetFeatureDefinition(container, feature);
                result.AddRange(container.ResolveDependencies(featureDefinition));
                result.Add(featureDefinition);
            }
        }

        /// <summary>
        /// Gets feature definition by feature instance
        /// </summary>
        /// <param name="container"> The container instance. </param>
        /// <param name="feature"> The feature instance. </param>
        /// <returns>
        /// Feature definitions
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if container doesn't support feature
        /// </exception>
        private FeatureDefinition GetFeatureDefinition(IContainer container, Feature feature)
        {
            if (!container.Features.Any(f => f.Name == feature.Name))
            {
                throw new InvalidOperationException(
                    string.Format("Container doesn't support {0} feature", feature.Name));
            }

            return container.Features.Single(f => f.Name == feature.Name);
        }

        /// <summary>
        /// Adds feature to html document
        /// </summary>
        /// <param name="document"> Target html document. </param>
        /// <param name="head"> Head html node. </param>
        /// <param name="feature"> Feature definition. </param>
        private void AddFeature(HtmlDocument document, HtmlNode head, FeatureDefinition feature)
        {
            var scripts = feature.GetGadgetScripts();
            foreach (var scriptDefinition in scripts)
            {
                HtmlNode script = document.CreateElement("script");
                script.Attributes.Add("type", "text/javascript");
                switch (scriptDefinition.Type)
                {
                    case ScriptContentType.Inline:
                        script.InnerHtml = scriptDefinition.Content;
                        break;
                    case ScriptContentType.Local:
                    case ScriptContentType.Remote:
                        script.Attributes.Add("src", scriptDefinition.Source);
                        break;
                    case ScriptContentType.Resource:
                        if (string.IsNullOrEmpty(scriptDefinition.Source))
                        {
                            Trace.Warn(TraceCategory, string.Format("some scripts of feature '{0}' cannot be resolved", feature.Name));
                            continue; 
                        }

                        script.Attributes.Add("src", scriptDefinition.Source);
                        break;
                }

                head.AppendChild(script);
            }
        }
    }
}
