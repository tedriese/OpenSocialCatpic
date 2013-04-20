// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupRepository.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Data.EntityFramework.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;

    using Catpic.Social;
    using Catpic.Social.Groups;
    using Catpic.Utils;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class EntityGroup : Group
    {
        public ICollection<EntityGroupCollection> EntityGroupCollections { get; set; }
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class EntityGroupCollection : EntityCollection<EntityGroup>
    {
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GroupRepository : IRepository<EntityGroup>
    {

                /// <summary>
        /// Connection string
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupRepository"/> class.
        /// </summary>
        /// <param name="connectionString"> Connection string. </param>
        public GroupRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }

        #region Implementation of IRepository<Group>

        /// <summary>
        /// Returns IQueryable object
        /// </summary>
        /// <returns> IQueryable object</returns>
        public IQueryable GetQueryable()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds entity to repository
        /// </summary>
        /// <param name="userId"> User id</param>
        /// <param name="collectionId"> Collection id</param>
        /// <param name="entity"> entity instance </param>
        /// <returns> Async task. </returns>
        public Task<EntityGroup> AddEntityAsync(string userId, string collectionId, EntityGroup entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates entity in repository
        /// </summary>
        /// <param name="userId"> User id</param>
        /// <param name="collectionId"> Collection id</param>
        /// <param name="entity"> entity instance </param>
        /// <returns> Async task. </returns>
        public Task<EntityGroup> UpdateEntityAsync(string userId, string collectionId, EntityGroup entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes entity in repository
        /// </summary>
        /// <param name="userId"> User id</param>
        /// <param name="collectionId"> Collection id</param>
        /// <param name="entity"> entity instance </param>
        /// <returns> Async task. </returns>
        public Task<EntityGroup> DeleteEntityAsync(string userId, string collectionId, EntityGroup entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add collection to repository
        /// </summary>
        /// <param name="collection"> The collection. </param>
        /// <returns> Async task.   </returns>
        public Task<string> AddCollectionAsync(EntityCollection<EntityGroup> collection)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates collection to repository
        /// </summary>
        /// <param name="collection"> The collection. </param>
        /// <returns> Async task.   </returns>
        public Task<string> UpdateCollectionAsync(EntityCollection<EntityGroup> collection)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes collection to repository
        /// </summary>
        /// <param name="userId"> User id </param>
        /// <param name="id"> Collection id. </param>
        /// <returns> Async task.  </returns>
        public Task<string> DeleteCollectionAsync(string userId, string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns collection of entries which match expression. NOTE: anonynmous type are possible here.
        /// </summary>
        /// <param name="expression"> Select expression. </param>
        /// <returns> Collection of entries. </returns>
        public Task<IEnumerable<object>> Select(Expression expression)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
