// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepository.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Defines entites repository behavior
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Social
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines entites repository behavior
    /// </summary>
    /// <typeparam name="T"> Entity type </typeparam>
    public interface IRepository<T> where T : IIdentityField
    {
        /// <summary>
        /// Returns IQueryable object
        /// </summary>
        /// <returns> IQueryable object</returns>
        IQueryable GetQueryable();

        /// <summary>
        /// Adds entity to repository
        /// </summary>
        /// <param name="userId"> User id</param>
        /// <param name="collectionId"> Collection id</param>
        /// <param name="entity"> entity instance </param>
        /// <returns> Async task. </returns>
        Task<T> AddEntityAsync(string userId, string collectionId, T entity);

        /// <summary>
        /// Updates entity in repository
        /// </summary>
        /// <param name="userId"> User id</param>
        /// <param name="collectionId"> Collection id</param>
        /// <param name="entity"> entity instance </param>
        /// <returns> Async task. </returns>
        Task<T> UpdateEntityAsync(string userId, string collectionId, T entity);

        /// <summary>
        /// Deletes entity in repository
        /// </summary>
        /// <param name="userId"> User id</param>
        /// <param name="collectionId"> Collection id</param>
        /// <param name="entity"> entity instance </param>
        /// <returns> Async task. </returns>
        Task<T> DeleteEntityAsync(string userId, string collectionId, T entity);

        /// <summary>
        /// Add collection to repository
        /// </summary>
        /// <param name="collection"> The collection. </param>
        /// <returns> Async task.   </returns>
        Task<string> AddCollectionAsync(EntityCollection<T> collection);

        /// <summary>
        /// Updates collection to repository
        /// </summary>
        /// <param name="collection"> The collection. </param>
        /// <returns> Async task.   </returns>
        Task<string> UpdateCollectionAsync(EntityCollection<T> collection);

        /// <summary>
        /// Removes collection to repository
        /// </summary>
        /// <param name="userId"> User id </param>
        /// <param name="id"> Collection id. </param>
        /// <returns> Async task.  </returns>
        Task<string> DeleteCollectionAsync(string userId, string id);

        /// <summary>
        /// Returns collection of entries which match expression. NOTE: anonynmous type are possible here.
        /// </summary>
        /// <param name="expression"> Select expression. </param>
        /// <returns> Collection of entries. </returns>
        Task<IEnumerable<object>> Select(Expression expression);
    }
}
