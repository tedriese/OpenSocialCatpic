// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISecurityRequestHandler.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents security handler
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Security
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Catpic.Gadgets.Proxies;

    /// <summary>
    /// Represents security handler
    /// </summary>
    public interface ISecurityRequestHandler
    {
        /// <summary>
        /// Processes security calls and calls original handler
        /// </summary>
        /// <param name="context"> Proxy context. </param>
        /// <param name="originalHandler"> Original Handler. </param>
        /// <returns> Async task. </returns>
        Task ProcessRequest(ProxyContext context, Func<ProxyContext, IDictionary<string, string>, string, Task> originalHandler);

        /// <summary>
        /// Processes security callback
        /// </summary>
        /// <param name="context"> Proxy context. </param>
        /// <returns> Async task. </returns>
        Task ProcessCallback(ProxyContext context);
    }
}