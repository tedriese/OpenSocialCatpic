// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContainerProvider.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents behavior of container provider
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Containers
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents behavior of container provider
    /// </summary>
    public interface IContainerProvider
    {
        /// <summary>
        /// Gets container list.
        /// </summary>
        IEnumerable<IContainer> Containers { get; }

        /// <summary>
        /// Gets container by name.
        /// </summary>
        /// <param name="name"> The name of container. </param>
        /// <returns> Container instance.</returns>
        IContainer GetContainer(string name);
    }
}
