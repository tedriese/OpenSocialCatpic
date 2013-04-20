// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConcatProxy.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents concat proxy which processes concat requests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Proxies
{
    using System.Threading.Tasks;

    /// <summary>
    /// Represents concat proxy which processes concat requests
    /// </summary>
    public interface IConcatProxy
    {
        /// <summary>
        /// Render scripts using provided gadget context.
        /// </summary>
        /// <param name="context">Gadget context.</param>
        /// <returns>Async context</returns>
        Task RenderScriptAsync(GadgetContext context);
    }
}
