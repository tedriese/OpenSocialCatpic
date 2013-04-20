// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityEntryRepository.cs" company="Catpic Software">
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
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;

    using Catpic.Data.EntityFramework.Helpers;
    using Catpic.Social;
    using Catpic.Social.Activities;
    using Catpic.Utils;
    using Catpic.Utils.Reflection;

    /// <summary>
    /// EF's activity entry
    /// </summary>
    public class EntityActivityEntry : ActivityEntry
    {
        [ForeignKey("UserId")]
        public EntityPerson Person { get; set; }

        /// <summary>
        /// Gets or sets EntityActivityEntryCollections.
        /// </summary>
        public ICollection<EntityActivityEntryCollection> EntityActivityEntryCollections { get; set; }
    }

    /// <summary>
    /// Represents activity entries collection as EF doesn't support generic types
    /// </summary>
    public class EntityActivityEntryCollection : EntityCollection<EntityActivityEntry>
    {
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ActivityEntryRepository : IRepository<EntityActivityEntry>
    {
         /// <summary>
        /// Connection string
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityEntryRepository"/> class.
        /// </summary>
        /// <param name="connectionString"> Connection string. </param>
        public ActivityEntryRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }

        #region Implementation of IRepository<ActivityEntry>

        /// <summary>
        /// Returns IQueryable object
        /// </summary>
        /// <returns> IQueryable object</returns>
        public IQueryable GetQueryable()
        {
            return CatpicContext.Current(this._connectionString).ActivityEntryCollections.AsQueryable();
        }

        /// <summary>
        /// Adds entity to repository
        /// </summary>
        /// <param name="userId"> User id</param>
        /// <param name="collectionId"> Collection id</param>
        /// <param name="entity"> entity instance </param>
        /// <returns> Async task. </returns>
        public Task<EntityActivityEntry> AddEntityAsync(string userId, string collectionId, EntityActivityEntry entity)
        {
            entity.Id = Guid.NewGuid().ToString();

            // NOTE collectionId should be @self by spec
            var selfCollection = (from c in CatpicContext.Current(this._connectionString).ActivityEntryCollections
                                  where c.UserId == userId && c.Type == collectionId
                                  select c).Single();

            selfCollection.Entities.Add(entity);

            // NOTE update friend's collections
            this.AddToGroup(userId, SocialConsts.GroupIdFriends, entity);

            CatpicContext.Current(this._connectionString).SaveChanges();
            return AsyncHelper.GetEmptyTask(entity);
        }

        /// <summary>
        /// Updates entity in repository
        /// </summary>
        /// <param name="userId"> User id</param>
        /// <param name="collectionId"> Collection id</param>
        /// <param name="entity"> entity instance </param>
        /// <returns> Async task. </returns>
        public Task<EntityActivityEntry> UpdateEntityAsync(string userId, string collectionId, EntityActivityEntry entity)
        {
            var activity = CatpicContext.Current(this._connectionString).Activities.Single(m => m.Id == entity.Id);

            PropertyHelper.CopyPropertyValues(entity, activity);
            
            CatpicContext.Current(this._connectionString).SaveChanges();
            return AsyncHelper.GetEmptyTask(activity); 
        }

        /// <summary>
        /// Deletes entity in repository
        /// </summary>
        /// <param name="userId"> User id</param>
        /// <param name="collectionId"> Collection id</param>
        /// <param name="entity"> entity instance </param>
        /// <returns> Async task. </returns>
        public Task<EntityActivityEntry> DeleteEntityAsync(string userId, string collectionId, EntityActivityEntry entity)
        {
            var result = CatpicContext.Current(this._connectionString).Activities.Single(a => a.Id == entity.Id);

            CatpicContext.Current(this._connectionString).Entry(result).State = EntityState.Deleted;
            CatpicContext.Current(this._connectionString).SaveChanges();

            return AsyncHelper.GetEmptyTask(entity);
        }

        /// <summary>
        /// Add collection to repository
        /// </summary>
        /// <param name="collection"> The collection. </param>
        /// <returns> Async task.   </returns>
        public Task<string> AddCollectionAsync(EntityCollection<EntityActivityEntry> collection)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates collection to repository
        /// </summary>
        /// <param name="collection"> The collection. </param>
        /// <returns> Async task.   </returns>
        public Task<string> UpdateCollectionAsync(EntityCollection<EntityActivityEntry> collection)
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
            IEnumerable<object> result = new QueryTranslator<object>(CatpicContext.Current(this._connectionString).ActivityEntryCollections, expression).AsEnumerable();
            return AsyncHelper.GetEmptyTask(result);
        }

        #endregion

        /// <summary>
        /// Adds entity to specific group
        /// </summary>
        /// <param name="userId"> The user id. </param>
        /// <param name="collectionId"> The collection id. </param>
        /// <param name="entity"> The entity. </param>
        private void AddToGroup(string userId, string collectionId, EntityActivityEntry entity)
        {
            IEnumerable<string> friendIds = (from c in CatpicContext.Current(this._connectionString).PersonCollections
                                             where c.UserId == userId && c.Type == collectionId
                                             select c.Entities).SelectMany(p => p).Select(p => p.Id);

            // add to friend's activity streams collection
            foreach (var friendId in friendIds)
            {
                var friendCollection = (from c in CatpicContext.Current(this._connectionString).ActivityEntryCollections
                                        where c.UserId == friendId && c.Type == collectionId
                                        select c).Single();
                friendCollection.Entities.Add(entity);
            }
        }
    }
}
