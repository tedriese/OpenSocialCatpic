// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonRepository.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Provides people repository using EntityFramework as underlying storage
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
    using System.Threading.Tasks;

    using Catpic.Data.EntityFramework.Helpers;
    using Catpic.Social;
    using Catpic.Social.People;
    using Catpic.Utils;
    using Catpic.Utils.Reflection;

    /// <summary>
    /// Represent person for EF
    /// </summary>
    public class EntityPerson : Person
    {
        /// <summary>
        /// Gets or sets PersonCollections.
        /// </summary>
        public ICollection<EntityPersonCollection> PersonCollections { get; set; }

        /// <summary>
        /// Gets or sets MessageCollections.
        /// </summary>
        [ForeignKey("UserId")]
        public ICollection<EntityMessageCollection> MessageCollections { get; set; }

        /// <summary>
        /// Gets or sets ActivityEntryCollections.
        /// </summary>
        [ForeignKey("UserId")]
        public ICollection<EntityActivityEntryCollection> ActivityEntryCollections { get; set; }
    }

    /// <summary>
    /// Represents people collection as EF doesn't support generic types
    /// </summary>
    public class EntityPersonCollection : EntityCollection<EntityPerson>
    {
    }

    /// <summary>
    /// Provides people repository using EF as underlying storage
    /// </summary>
    public class PersonRepository : IRepository<EntityPerson>
    {
        /// <summary>
        /// Connection string
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonRepository"/> class.
        /// </summary>
        /// <param name="connectionString"> Connection string. </param>
        public PersonRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }

        #region Implementation of IRepository<Person>

        /// <summary>
        /// Returns IQueryable object
        /// </summary>
        /// <returns> IQueryable object</returns>
        public IQueryable GetQueryable()
        {
            return CatpicContext.Current(this._connectionString).PersonCollections.AsQueryable();
        }

        /// <summary>
        /// Creates relationships between entities
        /// </summary>
        /// <param name="userId"> User id</param>
        /// <param name="collectionId"> Collection id</param>
        /// <param name="entity"> entity instance </param>
        /// <returns> Async task. </returns>
        public Task<EntityPerson> AddEntityAsync(string userId, string collectionId, EntityPerson entity)
        {
            var collection = (from c in CatpicContext.Current(this._connectionString).PersonCollections
                              where c.UserId == userId && c.Type == collectionId
                              select c).Single();

            collection.Entities.Add(entity);
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
        public Task<EntityPerson> UpdateEntityAsync(string userId, string collectionId, EntityPerson entity)
        {
            var person = CatpicContext.Current(this._connectionString)
                .Persons.Single(p => p.Id == entity.Id);
            PropertyHelper.CopyPropertyValues(entity, person);
            CatpicContext.Current(this._connectionString).SaveChanges();
            return AsyncHelper.GetEmptyTask(entity);
        }

        /// <summary>
        /// Deletes relationship in repository
        /// </summary>
        /// <param name="userId"> User id</param>
        /// <param name="collectionId"> Collection id</param>
        /// <param name="entity"> entity instance </param>
        /// <returns> Async task. </returns>
        public Task<EntityPerson> DeleteEntityAsync(string userId, string collectionId, EntityPerson entity)
        {
            // NOTE special case: delete person
            if (collectionId == SocialConsts.GroupIdSelf)
            {
                var result = CatpicContext.Current(this._connectionString).Persons.Single(a => a.Id == entity.Id);
                CatpicContext.Current(this._connectionString).Entry(result).State = EntityState.Deleted;
                CatpicContext.Current(this._connectionString).SaveChanges();

                return AsyncHelper.GetEmptyTask(entity);
            }

            // delete relationship
            var collection = (from c in CatpicContext.Current(this._connectionString).PersonCollections
                              where c.UserId == userId && c.Type == collectionId
                              select c).Single();

            var person = collection.Entities.Single(p => p.Id == entity.Id);

            collection.Entities.Remove(person);
            CatpicContext.Current(this._connectionString).SaveChanges();
            return AsyncHelper.GetEmptyTask(person);
        }

        /// <summary>
        /// Add collection to repository
        /// </summary>
        /// <param name="collection"> The collection. </param>
        /// <returns> Async task.   </returns>
        public Task<string> AddCollectionAsync(EntityCollection<EntityPerson> collection)
        {
            this.AddCollectionWithEntities(collection, new List<EntityPerson>(), this._connectionString);
            CatpicContext.Current(this._connectionString).SaveChanges();
            return AsyncHelper.GetEmptyTask(collection.Title);
        }

        /// <summary>
        /// Updates collection to repository
        /// </summary>
        /// <param name="collection"> The collection. </param>
        /// <returns> Async task.   </returns>
        public Task<string> UpdateCollectionAsync(EntityCollection<EntityPerson> collection)
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
            IEnumerable<object> result = new QueryTranslator<object>(CatpicContext.Current(this._connectionString).PersonCollections, expression).AsEnumerable();
            return AsyncHelper.GetEmptyTask(result);
        }

        #endregion

        /// <summary>
        /// Adds person collection with entities
        /// </summary>
        /// <param name="collection"> The collection. </param>
        /// <param name="persons"> The persons. </param>
        /// <param name="connectionString"> The connection string. </param>
        private void AddCollectionWithEntities(EntityCollection<EntityPerson> collection, ICollection<EntityPerson> persons, string connectionString)
        {
            var copy = new EntityPersonCollection();
            copy.Id = Guid.NewGuid().ToString();
            copy.Type = collection.Type;
            copy.UserId = collection.UserId;
            copy.Title = collection.Title;
            copy.Entities = persons;
            CatpicContext.Current(connectionString).PersonCollections.Add(copy);
        }
    }
}
