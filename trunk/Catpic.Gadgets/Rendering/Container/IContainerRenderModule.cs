// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContainerRenderModule.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents single module of container rendering pipeline
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Rendering.Container
{
    using System.Text;
    using System.Threading.Tasks;

    using Catpic.Gadgets.Containers;

    /// <summary>
    /// Represents single module of container rendering pipeline
    /// </summary>
    public interface IContainerRenderModule
    {
        /// <summary>
        /// Renders container specific content into StringBuilder object
        /// </summary>
        /// <param name="container"> Container instance. </param>
        /// <param name="context"> Container context. </param>
        /// <param name="content"> Output content builder. </param>
        /// <returns> Async Task</returns>
        Task RenderAsync(IContainer container, ContainerContext context, StringBuilder content);
    }
}
