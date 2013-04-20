// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GadgetRenderPipeline.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Gadget render pipeline which renders gadget's content using configured rendering modules
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Rendering.Gadget
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Catpic.Gadgets.Containers;
    using Catpic.Utils;
    using Catpic.Utils.Diagnostic;

    using HtmlAgilityPack;

    /// <summary>
    /// Gadget render pipeline which renders gadget's content using configured rendering modules
    /// </summary>
    public class GadgetRenderPipeline : IGadgetRenderPipeline
    {
        /// <summary>
        /// Trace category
        /// </summary>
        private const string TraceCategory = "render.gadget";

        /// <summary>
        /// Trace instance
        /// </summary>
        private static readonly ITrace Trace = TraceFactory.GetTrace();

        /// <summary>
        /// Render modules list
        /// </summary>
        private readonly IEnumerable<IGadgetRenderModule> _renderModules;

        /// <summary>
        /// Initializes a new instance of the <see cref="GadgetRenderPipeline"/> class.
        /// </summary>
        /// <param name="renderModules"> Render module list. </param>
        public GadgetRenderPipeline(IEnumerable<IGadgetRenderModule> renderModules)
        {
            this._renderModules = renderModules;
        }

        /// <summary>
        /// Renders gadget content into writer
        /// </summary>
        /// <param name="container">Container instance</param>
        /// <param name="gadget">Gadget instance</param>
        /// <param name="writer">Output writer</param>
        /// <returns>Async Task</returns>
        public Task RenderAsync(IContainer container, Catpic.Gadgets.Gadget gadget, TextWriter writer)
        {
            Trace.Debug(TraceCategory, string.Format("begin {0}", gadget.Context.Uri));
            HtmlDocument hDoc = this.CreateHtmlDocument(gadget.Context.RenderMode);

            // TODO: it would be great to collect all independed async tasks into one bundle and execute it at the same time
            return AsyncHelper.Iterate(this.CreatePipeline(container, gadget, hDoc))
                .ContinueWith(t =>
                {
                    hDoc.DocumentNode.WriteTo(writer);
                    Trace.Debug(TraceCategory, string.Format("end {0}", gadget.Context.Uri));
                });
        }

        /// <summary>
        /// Create rendering pipeline.
        /// </summary>
        /// <param name="container"> Container instance. </param>
        /// <param name="gadget"> Gadget instance. </param>
        /// <param name="hDoc"> Target html documetn. </param>
        /// <returns> Async task list</returns>
        private IEnumerable<Task> CreatePipeline(IContainer container, Catpic.Gadgets.Gadget gadget, HtmlDocument hDoc)
        {
            foreach (var gadgetRenderModule in this._renderModules)
            {
                yield return gadgetRenderModule.RenderAsync(container, gadget, hDoc);
            }
        }

        /// <summary>
        /// Creates html document TODO: create once and clone object?
        /// </summary>
        /// <param name="renderMode"> Render mode. </param>
        /// <returns> Target html document </returns>
        private HtmlDocument CreateHtmlDocument(RenderModeType renderMode)
        {
            var hDoc = new HtmlDocument();
            if (renderMode == RenderModeType.Iframe)
            {
                // create html document with head and body
                var htmlNode = hDoc.CreateElement("html");
                hDoc.DocumentNode.AppendChild(htmlNode);

                var hHead = hDoc.CreateElement("head");
                htmlNode.ChildNodes.Append(hHead);

                var hBody = hDoc.CreateElement("body");
                htmlNode.ChildNodes.Append(hBody);
            }
            else
            {
                // just create single wrapper element
                var divNode = hDoc.CreateElement("div");
                hDoc.DocumentNode.AppendChild(divNode);
            }

            return hDoc;
        }
    }
}
