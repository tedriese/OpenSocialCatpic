// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProxyContext.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents proxy request call context
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Proxies
{
    using System.Web;

    using Catpic.Gadgets.Security;

    /// <summary>
    /// Represents proxy request call context
    /// </summary>
    public class ProxyContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyContext"/> class.
        /// </summary>
        /// <param name="context"> Http context. </param>
        /// <param name="securityToken"> Security token. </param>
        public ProxyContext(HttpContextBase context, ISecurityToken securityToken)
        {
            this.Http = context;
            this.SecurityToken = securityToken;
        }

        /// <summary>
        /// Gets Http context.
        /// </summary>
        public HttpContextBase Http { get; private set; }

        /// <summary>
        /// Gets SecurityToken.
        /// </summary>
        public ISecurityToken SecurityToken { get; private set; }
    }
}
