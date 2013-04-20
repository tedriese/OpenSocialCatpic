// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextFactory.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Default implementation of context factory which produces gadget and container-specific contexts
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets
{
    using System.Web;

    using Catpic.Gadgets.Proxies;
    using Catpic.Gadgets.Security;

    /// <summary>
    /// Default implementation of context factory which produces gadget and container-specific contexts.
    /// </summary>
    public class ContextFactory : IContextFactory
    {
        /// <summary>
        /// Security token factory instance.
        /// </summary>
        private readonly ISecurityTokenFactory _securityTokenFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextFactory"/> class.
        /// </summary>
        /// <param name="securityTokenFactory"> The security token factory. </param>
        public ContextFactory(ISecurityTokenFactory securityTokenFactory)
        {
            this._securityTokenFactory = securityTokenFactory;
        }

        /// <summary>
        /// Creates gadget context from http context.
        /// </summary>
        /// <param name="context"> Http context instance. </param>
        /// <returns> Gadget context </returns>
        public GadgetContext CreateGadgetContext(HttpContextBase context)
        {
            var token = this._securityTokenFactory.Create(context);
            return new GadgetContext(context, token);
        }

        /// <summary>
        /// Creates proxy context from http context.
        /// </summary>
        /// <param name="context"> Http context instance. </param>
        /// <returns> Proxy context </returns>
        public ProxyContext CreateProxyContext(HttpContextBase context)
        {
            var token = this._securityTokenFactory.Create(context);
            return new ProxyContext(context, token);
        }

        /// <summary>
        /// Creates container context from http context.
        /// </summary>
        /// <param name="context"> Http context instance. </param>
        /// <returns> Container context</returns>
        public ContainerContext CreateContainerContext(HttpContextBase context)
        {
            var token = this._securityTokenFactory.Create(context);
            return new ContainerContext(context, token);
        }
    }
}
