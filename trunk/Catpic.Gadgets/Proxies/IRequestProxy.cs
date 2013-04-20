// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRequestProxy.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Defines behavior of request proxy
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Proxies
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines behavior of request proxy
    /// </summary>
    public interface IRequestProxy
    {
        /// <summary>
        /// Process gadgets.makeRequest
        /// </summary>
        /// <param name="context"> Proxy context. </param>
        /// <param name="headers"> Http headers map. </param>
        /// <param name="queryString"> Additional query String. </param>
        /// <returns> Async task. </returns>
        Task MakeRequestAsync(ProxyContext context, IDictionary<string, string> headers, string queryString);

        /// <summary>
        /// Process proxy request
        /// </summary>
        /// <param name="context"> Proxy context. </param>
        /// <param name="headers"> Http headers map. </param>
        /// <param name="queryString"> Additional query String. </param>
        /// <returns> Async task. </returns>
        Task ProxyRequestAsync(ProxyContext context, IDictionary<string, string> headers, string queryString);
    }
}
