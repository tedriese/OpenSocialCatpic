// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureContainerRenderModule.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Renders container features
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Rendering.Container
{
    using System;
    using System.Text;
    using System.Threading.Tasks;

    using Catpic.Gadgets.Containers;
    using Catpic.Gadgets.Format;
    using Catpic.Utils;
    using Catpic.Utils.Diagnostic;

    /// <summary>
    /// Renders container features.
    /// </summary>
    public class FeatureContainerRenderModule : IContainerRenderModule
    {
        /// <summary>
        /// Trace category
        /// </summary>
        private const string TraceCategory = "container.render.feature";

        /// <summary>
        /// Trace instance
        /// </summary>
        private static readonly ITrace Trace = TraceFactory.GetTrace();

        /// <summary>
        /// Renders container specific content into StringBuilder object
        /// </summary>
        /// <param name="container"> Container instance. </param>
        /// <param name="context"> Container context. </param>
        /// <param name="content"> Output content builder. </param>
        /// <returns> Async Task</returns>
        public Task RenderAsync(IContainer container, ContainerContext context, StringBuilder content)
        {
            Trace.Debug(TraceCategory, "begin");
            if (context.Action == "initialize")
            {
                RenderContent(container, content);
            }

            Trace.Debug(TraceCategory, "end");

            return AsyncHelper.GetEmptyTask();
        }

        /// <summary>
        /// Renders content
        /// </summary>
        /// <param name="container"> Container instance. </param>
        /// <param name="content"> The content output builder. </param>
        private static void RenderContent(IContainer container, StringBuilder content)
        {
            foreach (var feature in container.ContainerFeatures)
            {
                var containerScripts = feature.GetContainerScripts();
                foreach (var scriptDefinition in containerScripts)
                {
                    // get script content and write it to writer
                    var closure = scriptDefinition;
                    var scriptContent = string.Empty;
                    switch (closure.Type)
                    {
                        case ScriptContentType.Local:
                        case ScriptContentType.Inline:
                            scriptContent = closure.Content;
                            break;
                        default:
                            throw new NotImplementedException(
                                "Only local or inline scripts are supported during container initialization process");
                    }

                    content.Append(scriptContent);
                }
            }
        }
    }
}
