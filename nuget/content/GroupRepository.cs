// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageCollectionRepository.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Defines the MessageRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Host.Engine.Social
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Catpic.Social;
    using Catpic.Social.Groups;
    using Catpic.Utils;

    /// <summary>
    /// Represents stub message repository
    /// </summary>
    public class GroupRepository: IRepository<Group>
    {
        private readonly IList<EntityCollection<Group>> _groups;
        private IQueryable<EntityCollection<Group>> _queryable;
        public GroupRepository(IList<EntityCollection<Group>> groups)
        {
            this._groups = groups;
            _queryable = this._groups.AsQueryable();
        }

        #region Implementation of IMessageCollectionRepository<Message>

        public IQueryable GetQueryable()
        {
            return _queryable;
        }

        public Task<Group> AddEntityAsync(string userId, string collectionId, Group group)
        {
            // NOTE: support only single collection id
            var collection = this._groups.Single(c => c.UserId == userId);

            // NOTE: predefined unit test value (ok, as this implementation is built for unit testing)
            group.Id = group.Title == "test title1" ? "testID" : Guid.NewGuid().ToString();
            (collection.Entities as IList<Group>).Add(group);
            _queryable = this._groups.AsQueryable();
            return AsyncHelper.GetEmptyTask(group);
        }

        public Task<Group> UpdateEntityAsync(string userId, string collectionId, Group entity)
        {
            var collection = this._groups.Single(c => c.UserId == userId);
            var group = collection.Entities.Single(g => g.Id == entity.Id);
            group.Title = entity.Title;
            return AsyncHelper.GetEmptyTask(group);
        }

        public Task<Group> DeleteEntityAsync(string userId, string collectionId, Group entity)
        {
            var collection = this._groups.Single(c => c.UserId == userId);
            var group = collection.Entities.Single(g => g.Id == entity.Id);
            (collection.Entities as List<Group>).Remove(group);
            return AsyncHelper.GetEmptyTask(group);
        }

        public Task<string> AddCollectionAsync(EntityCollection<Group> collection)
        {
            throw new NotSupportedException();
        }

        public Task<string> UpdateCollectionAsync(EntityCollection<Group> collection)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteCollectionAsync(string userId, string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<object>> Select(Expression expression)
        {
            var query = _queryable.Provider.CreateQuery(expression);
            return AsyncHelper.GetEmptyTask(query as IEnumerable<object>);
        }

        #endregion


    }
}