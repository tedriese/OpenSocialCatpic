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
    using Catpic.Social.Messages;
    using Catpic.Utils;

    /// <summary>
    /// Represents stub message repository
    /// </summary>
    public class MessageRepository: IRepository<Message>
    {
        private readonly IList<EntityCollection<Message>> _messages;
        private IQueryable<EntityCollection<Message>> _queryable;
        public MessageRepository(IList<EntityCollection<Message>> messages)
        {
            _messages = messages;
            _queryable = _messages.AsQueryable();
        }

        #region Implementation of IMessageCollectionRepository<Message>

        public IQueryable GetQueryable()
        {
            return _queryable;
        }

        public Task<Message> AddEntityAsync(string userId, string collectionId, Message message)
        {
            // NOTE: support only single recipient and collection id
            var collection = _messages.Single(c => c.UserId == message.Recipients.Single() && c.Type == message.CollectionIds.Single());

            // NOTE: For unit testing
            if (message.Title == "Message from UnitTest")
            {
                message.TimeSent = "9/8/2012 11:17:16 PM";
                message.Updated = "9/8/2012 11:17:16 PM";
            }
            else
            {
                message.TimeSent = DateTime.Now.ToString();
                message.Updated = DateTime.Now.ToString();
            }
            message.SenderId = userId;

            (collection.Entities as IList<Message>).Add(message);
            _queryable = _messages.AsQueryable();
            return AsyncHelper.GetEmptyTask(message);
        }

        public Task<Message> UpdateEntityAsync(string userId, string collectionId, Message entity)
        {
            var collection = _messages.Single(c => c.UserId == userId && c.Type == collectionId);

            var message = collection.Entities.Single(m => m.Id == entity.Id);
            (collection.Entities as List<Message>).Remove(message);
            (collection.Entities as List<Message>).Add(entity);
            return AsyncHelper.GetEmptyTask(entity);
        }

        public Task<Message> DeleteEntityAsync(string userId, string collectionId, Message entity)
        {
            var collection = _messages.Single(c => c.UserId == userId && c.Type == collectionId);

            var message = collection.Entities.Single(m => m.Id == entity.Id);
            (collection.Entities as List<Message>).Remove(message);

            return AsyncHelper.GetEmptyTask(entity);

        }

        public Task<string> AddCollectionAsync(EntityCollection<Message> collection)
        {
            var id = collection.Type ?? Guid.NewGuid().ToString();
            _messages.Add(
                new EntityCollection<Message>()
                    {
                    Type = id,
                    Title = collection.Title,
                    UserId = collection.UserId
                });
            _queryable = _messages.AsQueryable();
            return AsyncHelper.GetEmptyTask(id);
        }

        public Task<string> UpdateCollectionAsync(EntityCollection<Message> collection)
        {
            var cm = _messages.Single(c => c.Type == collection.Type && c.UserId == collection.UserId);
            cm.Title = collection.Title;
            return AsyncHelper.GetEmptyTask(cm.Title);
        }

        public Task<string> DeleteCollectionAsync(string userId, string id)
        {
            var collection = _messages.Single(c => c.Type == id && c.UserId == userId);
            _messages.Remove(collection);
            return AsyncHelper.GetEmptyTask(id);
        }

        public Task<IEnumerable<object>> Select(Expression expression)
        {
            var query = _queryable.Provider.CreateQuery(expression);
            return AsyncHelper.GetEmptyTask(query as IEnumerable<object>);
        }

        #endregion
    }
}