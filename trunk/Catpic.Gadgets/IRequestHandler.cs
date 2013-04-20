// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRequestHandler.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Facade for gadget processing
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets
{
    using System.Threading.Tasks;

    using Catpic.Gadgets.Proxies;

    /// <summary>
    /// Facade for gadget processing
    /// </summary>
    public interface IRequestHandler
    {
        /// <summary>
        /// Process container-specific requests
        /// </summary>
        /// <param name="context">Container context</param>
        /// <returns>Async task</returns>
        Task ContainerAsync(ContainerContext context);

        /// <summary>
        /// Process metadata requests
        /// </summary>
        /// <param name="context">Container context</param>
        /// <returns>Async task</returns>
        Task MetadataAsync(ContainerContext context);

        /// <summary>
        /// Creates gadget from context
        /// </summary>
        /// <param name="context">Gadget context</param>
        /// <returns>Async task</returns>
        Task CreateGadgetAsync(GadgetContext context);

        /// <summary>
        /// Processes gadgets.makeRequest
        /// </summary>
        /// <param name="context">Proxy context</param>
        /// <returns>Async task</returns>
        Task MakeRequestAsync(ProxyContext context);

        /// <summary>
        /// Proxies requests
        /// </summary>
        /// <param name="context">Proxy context</param>
        /// <returns>Async task</returns>
        Task ProxyRequestAsync(ProxyContext context);

        /// <summary>
        /// for OAuth functionality
        /// </summary>
        /// <param name="context">Proxy context</param>
        /// <returns>Async task</returns>
        Task SecurityCallbackAsync(ProxyContext context);

        /// <summary>
        /// Process concat scripts
        /// </summary>
        /// <param name="context">Gadget context</param>
        /// <returns> Async task</returns>
        Task ConcatScriptAsync(GadgetContext context);
    }
}
