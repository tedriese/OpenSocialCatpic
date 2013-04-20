// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainerProvider.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Provides container instances.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Containers
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides container instances.
    /// </summary>
    public class ContainerProvider : IContainerProvider
    {
        /// <summary>
        /// List of registered containers.
        /// </summary>
        private readonly IEnumerable<IContainer> _containers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerProvider"/> class.
        /// </summary>
        /// <param name="containers"> List of registered containers. </param>
        public ContainerProvider(IEnumerable<IContainer> containers)
        {
            this._containers = containers;
        }

        #region IContainerProvider members

        /// <summary>
        /// Gets full list of containers.
        /// </summary>
        public IEnumerable<IContainer> Containers
        {
            get { return this._containers; }
        }

        /// <summary>
        /// Gets container by name
        /// </summary>
        /// <param name="name"> Name of container. </param>
        /// <returns> Container instance </returns>
        public IContainer GetContainer(string name)
        {
            return this._containers.Single(c => c.Name == name);
        }

        #endregion
    }
}