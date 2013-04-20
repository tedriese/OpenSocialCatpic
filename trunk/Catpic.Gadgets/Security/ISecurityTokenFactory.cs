// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISecurityTokenFactory.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Defines behavior of security token factory
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Security
{
    using System.Collections.Generic;
    using System.Security.Principal;
    using System.Web;

    /// <summary>
    /// Defines behavior of security token factory
    /// </summary>
    public interface ISecurityTokenFactory
    {
        /// <summary>
        /// Creates security token from http context
        /// </summary>
        /// <param name="httpContext"> Http context. </param>
        /// <returns> Security token. </returns>
        ISecurityToken Create(HttpContextBase httpContext);

        /// <summary>
        /// Creates security token from User and property map
        /// </summary>
        /// <param name="user"> Current user. </param>
        /// <param name="properties"> Property map. </param>
        /// <returns>  Security token.  </returns>
        ISecurityToken Create(IPrincipal user, IDictionary<string, object> properties);

        /// <summary>
        /// Creates anonymous token
        /// </summary>
        /// <param name="httpContext"> Http context. </param>
        /// <returns> Security token. </returns>
        ISecurityToken CreateAnonymous(HttpContextBase httpContext);
    }
}
