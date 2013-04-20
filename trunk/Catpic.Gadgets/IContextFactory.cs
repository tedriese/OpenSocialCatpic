// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContextFactory.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Defines behavior of context factory
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets
{
    using System.Web;

    using Catpic.Gadgets.Proxies;

    /// <summary>
    /// Defines behavior of context factory
    /// </summary>
    public interface IContextFactory
    {
        /// <summary>
        /// Creates container context
        /// </summary>
        /// <param name="context">Http context</param>
        /// <returns>Container context</returns>
        ContainerContext CreateContainerContext(HttpContextBase context);

        /// <summary>
        /// Creates gadget context
        /// </summary>
        /// <param name="context">Http context</param>
        /// <returns>Gadget context</returns>
        GadgetContext CreateGadgetContext(HttpContextBase context);

        /// <summary>
        /// Creates proxy context
        /// </summary>
        /// <param name="context">Http context</param>
        /// <returns>Proxy context</returns>
        ProxyContext CreateProxyContext(HttpContextBase context);

    }
}