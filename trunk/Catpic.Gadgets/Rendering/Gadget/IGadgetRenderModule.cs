// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GadgetRenderModule.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents a module of GadgetRenderPipeline. Must be thread-safe
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Rendering.Gadget
{
    using System.Threading.Tasks;

    using Catpic.Gadgets.Containers;

    using HtmlAgilityPack;

    /// <summary>
    /// Represents a module of GadgetRenderPipeline. Must be thread-safe
    /// </summary>
    public interface IGadgetRenderModule
    {
        /// <summary>
        /// Renders gadget content into html document
        /// </summary>
        /// <param name="container">Container instance.</param>
        /// <param name="gadget">Gadget instance.</param>
        /// <param name="document">Target html document</param>
        /// <returns>Async task</returns>
        Task RenderAsync(IContainer container, Catpic.Gadgets.Gadget gadget, HtmlDocument document);
    }
}
