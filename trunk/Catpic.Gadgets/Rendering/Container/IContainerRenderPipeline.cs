// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContainerRenderPipeline.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents container rendering pipeline which renders container-specific content
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Rendering.Container
{
    using System.IO;
    using System.Threading.Tasks;

    using Catpic.Gadgets.Containers;

    /// <summary>
    /// Represents container rendering pipeline which renders container-specific content
    /// </summary>
    public interface IContainerRenderPipeline
    {
        /// <summary>
        /// Renders container specific content into StringBuilder object
        /// </summary>
        /// <param name="container"> Container instance. </param>
        /// <param name="context"> Context context. </param>
        /// <param name="writer"> Output writer. </param>
        /// <returns> Async task</returns>
        Task RenderAsync(IContainer container, ContainerContext context, TextWriter writer);
    }
}
