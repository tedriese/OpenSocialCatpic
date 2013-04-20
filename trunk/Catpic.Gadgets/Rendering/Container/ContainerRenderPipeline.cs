// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainerRenderPipeline.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Returns container init scripts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Rendering.Container
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    using Catpic.Gadgets.Containers;
    using Catpic.Utils;
    using Catpic.Utils.Diagnostic;

    /// <summary>
    /// Returns container init scripts.
    /// </summary>
    public class ContainerRenderPipeline : IContainerRenderPipeline
    {
        /// <summary>
        /// Trace category.
        /// </summary>
        private const string TraceCategory = "container.render";

        /// <summary>
        /// Trace instance.
        /// </summary>
        private static readonly ITrace Trace = TraceFactory.GetTrace();

        /// <summary>
        /// Render modules list.
        /// </summary>
        private readonly IEnumerable<IContainerRenderModule> _renderModules;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerRenderPipeline"/> class.
        /// </summary>
        /// <param name="renderModules"> The render modules list. </param>
        public ContainerRenderPipeline(IEnumerable<IContainerRenderModule> renderModules)
        {
            this._renderModules = renderModules;
        }

        #region IContainerRenderPipeline members

        /// <summary>
        /// Renders container specific content into StringBuilder object
        /// </summary>
        /// <param name="container"> Container instance. </param>
        /// <param name="context"> Context context. </param>
        /// <param name="writer"> Output writer. </param>
        /// <returns> Async task</returns>
        public Task RenderAsync(IContainer container, ContainerContext context, TextWriter writer)
        {
            Trace.Debug(TraceCategory, string.Format("begin"));
            var content = new StringBuilder(1024 * 100);
            return AsyncHelper.Iterate(this.CreatePipeline(container, context, content))
                .ContinueWith(t =>
                {
                    writer.Write(content);
                    Trace.Debug(TraceCategory, string.Format("end"));
                });
        }

        #endregion

        #region private members

        /// <summary>
        /// Creates async pipeline of tassk
        /// </summary>
        /// <param name="container"> Container instance. </param>
        /// <param name="context"> Context context. </param>
        /// <param name="content"> Output content builder. </param>
        /// <returns> Async task enumerable </returns>
        private IEnumerable<Task> CreatePipeline(IContainer container, ContainerContext context, StringBuilder content)
        {
            foreach (var containerRenderModule in this._renderModules)
            {
                Trace.Debug(TraceCategory, string.Format("queue {0}",  containerRenderModule));
                yield return containerRenderModule.RenderAsync(container, context, content);
            }
        }
        #endregion
    }
}
