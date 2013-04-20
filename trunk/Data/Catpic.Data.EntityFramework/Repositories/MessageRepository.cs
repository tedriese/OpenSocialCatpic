// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageRepository.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   TODO: Update summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Data.EntityFramework.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Catpic.Data.EntityFramework.Helpers;
    using Catpic.Social;
    using Catpic.Social.Messages;
    using Catpic.Utils;
    using Catpic.Utils.Reflection;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class EntityMessage : Message
    {
        public ICollection<EntityMessageCollection> EntityMessageCollections { get; set; }
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class EntityMessageCollection : EntityCollection<EntityMessage>
    {
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MessageRepository : IRepository<EntityMessage>
    {
        /// <summary>
        /// Connection string
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRepository"/> class.
        /// </summary>
        /// <param name="connectionString"> Connection string. </param>
        public MessageRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }

        #region Implementation of IRepository<Message>

        /// <summary>
        /// Returns IQueryable object
        /// </summary>
        /// <returns> IQueryable object</returns>
        public IQueryable GetQueryable()
        {
            return CatpicContext.Current(this._connectionString).MessageCollections.AsQueryable();
        }

        /// <summary>
        /// Adds entity to repository
        /// </summary>
        /// <param name="userId"> PersonId id</param>
        /// <param name="collectionId"> Collection id</param>
        /// <param name="entity"> entity instance </param>
        /// <returns> Async task. </returns>
        public Task<EntityMessage> AddEntityAsync(string userId, string collectionId, EntityMessage entity)
        {
            entity.Id = Guid.NewGuid().ToString();
            entity.Status = SocialConsts.NewMessageStatus;
            var senderCollection = (from c in CatpicContext.Current(this._connectionString).MessageCollections
                                    where c.UserId == userId && c.Type == collectionId
                                    select c).Single();
            
            // NOTE support only one recipient
            var recipient = entity.Recipients.First();
            var receiverCollection = (from c in CatpicContext.Current(this._connectionString).MessageCollections
                                      where c.UserId == recipient && c.Type == collectionId
                                      select c).Single();


            senderCollection.Entities.Add(entity);
            receiverCollection.Entities.Add(entity);

            CatpicContext.Current(this._connectionString).SaveChanges();
            return AsyncHelper.GetEmptyTask(entity);
        }

        /// <summary>
        /// Updates entity in repository
        /// </summary>
        /// <param name="userId"> PersonId id</param>
        /// <param name="collectionId"> Collection id</param>
        /// <param name="entity"> entity instance </param>
        /// <returns> Async task. </returns>
        public Task<EntityMessage> UpdateEntityAsync(string userId, string collectionId, EntityMessage entity)
        {
            var message = CatpicContext.Current(this._connectionString).Messages.Single(m => m.Id == entity.Id);

            PropertyHelper.CopyPropertyValues(entity, message);

            CatpicContext.Current(this._connectionString).SaveChanges();
            return AsyncHelper.GetEmptyTask(message); 
        }

        /// <summary>
        /// Deletes entity in repository
        /// </summary>
        /// <param name="userId"> PersonId id</param>
        /// <param name="collectionId"> Collection id</param>
        /// <param name="entity"> entity instance </param>
        /// <returns> Async task. </returns>
        public Task<EntityMessage> DeleteEntityAsync(string userId, string collectionId, EntityMessage entity)
        {
            var result = CatpicContext.Current(this._connectionString).Messages.Single(a => a.Id == entity.Id);

            CatpicContext.Current(this._connectionString).Entry(result).State = EntityState.Deleted;
            CatpicContext.Current(this._connectionString).SaveChanges();
            return AsyncHelper.GetEmptyTask(result); 
        }

        /// <summary>
        /// Add collection to repository
        /// </summary>
        /// <param name="collection"> The collection. </param>
        /// <returns> Async task.   </returns>
        public Task<string> AddCollectionAsync(EntityCollection<EntityMessage> collection)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates collection to repository
        /// </summary>
        /// <param name="collection"> The collection. </param>
        /// <returns> Async task. </returns>
        public Task<string> UpdateCollectionAsync(EntityCollection<EntityMessage> collection)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes collection to repository
        /// </summary>
        /// <param name="userId"> PersonId id </param>
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
            IEnumerable<object> result = new QueryTranslator<object>(CatpicContext.Current(this._connectionString).MessageCollections, expression).AsEnumerable();
            return AsyncHelper.GetEmptyTask(result);
        }

        #endregion
    }
}
