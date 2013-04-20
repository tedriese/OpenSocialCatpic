// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHostContainer.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Defines base IoC functionality required by CatpicConfigurator
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Web.Configuration
{
    using System;

    /// <summary>
    /// Defines base IoC functionality required by CatpicConfigurator
    /// </summary>
    public interface IHostContainer
    {
        /// <summary>
        /// Checks whethere type is registered in container
        /// </summary>
        /// <typeparam name="T"> Some type </typeparam>
        /// <returns> True if type is registered. </returns>
        bool IsRegistered<T>();

        /// <summary>
        /// Registers type
        /// </summary>
        /// <typeparam name="T"> Interface type </typeparam>
        ///  <typeparam name="TC"> Implementation type </typeparam>
        /// <returns> this instance. </returns>
        IHostContainer RegisterType<T, TC>();

        /// <summary>
        /// Registers instance
        /// </summary>
        /// <typeparam name="T"> Implementation of interface type</typeparam> 
        /// <param name="interface"> Interface type.   </param>
        /// <param name="instance"> Instance object.   </param>
        /// <returns> this instance. </returns>
        IHostContainer RegisterInstance<T>(Type @interface, T instance);

        /// <summary>
        /// Resolves types
        /// </summary>
        /// <typeparam name="T">Any type </typeparam>
        /// <returns> Resolved instance. </returns>
        T Resolve<T>();
    }
}
