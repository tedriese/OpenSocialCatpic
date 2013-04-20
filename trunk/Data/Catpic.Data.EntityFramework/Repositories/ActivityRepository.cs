// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityRepository.cs" company="Catpic Software">
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
    
    using System.Threading.Tasks;

    using Catpic.Social;
    using Catpic.Social.Activities;


    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class EntityActivity : Activity
    {
        public ICollection<EntityActivityCollection> EntityActivityCollections { get; set; }
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class EntityActivityCollection : EntityCollection<EntityActivity>
    {
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ActivityRepository : IRepository<EntityActivity>
    {
        /// <summary>
        /// Connection string
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityRepository"/> class.
        /// </summary>
        /// <param name="connectionString"> Connection string. </param>
        public ActivityRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }

        #region Implementation of IRepository<Activity>

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
        public Task<EntityActivity> AddEntityAsync(string userId, string collectionId, EntityActivity entity)
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
        public Task<EntityActivity> UpdateEntityAsync(string userId, string collectionId, EntityActivity entity)
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
        public Task<EntityActivity> DeleteEntityAsync(string userId, string collectionId, EntityActivity entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add collection to repository
        /// </summary>
        /// <param name="collection"> The collection. </param>
        /// <returns> Async task.   </returns>
        public Task<string> AddCollectionAsync(EntityCollection<EntityActivity> collection)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates collection to repository
        /// </summary>
        /// <param name="collection"> The collection. </param>
        /// <returns> Async task.   </returns>
        public Task<string> UpdateCollectionAsync(EntityCollection<EntityActivity> collection)
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
