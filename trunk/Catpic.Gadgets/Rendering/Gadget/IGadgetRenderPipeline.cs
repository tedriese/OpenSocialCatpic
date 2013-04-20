// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GadgetRenderPipeline.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Renders gadget content
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Rendering.Gadget
{
    using System.IO;
    using System.Threading.Tasks;

    using Catpic.Gadgets.Containers;

    /// <summary>
    /// Renders gadget content
    /// </summary>
    public interface IGadgetRenderPipeline
    {
        /// <summary>
        /// Renders gadget content into writer
        /// </summary>
        /// <param name="container">Container instance</param>
        /// <param name="gadget">Gadget instance</param>
        /// <param name="writer">Output writer</param>
        /// <returns>Async Task</returns>
        Task RenderAsync(IContainer container, Catpic.Gadgets.Gadget gadget, TextWriter writer);
    }
}
