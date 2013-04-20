// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHandler.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//  Message service
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Social.Messages
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Catpic.Gadgets.Security;
    using Catpic.Social.DTO;
    using Catpic.Social.Formatting;
    using Catpic.Utils;

    /// <summary>
    /// Message service
    /// </summary>
    /// <typeparam name="T"> Message entity </typeparam>
    public class MessageHandler<T> : SocialHandler where T : IIdentityField
    {
        /// <summary>
        /// Message repository.
        /// </summary>
        private readonly IRepository<T> _messageRepository;

        /// <summary>
        /// Message expression factory
        /// </summary>
        private readonly SocialExpressionFactory<T> _expressionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandler{T}"/> class. 
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <param name="messageRepository"> Messages repository. </param>
        /// <param name="expressionFactory"> The expression factory. </param>
        public MessageHandler(
            string name,
            IRepository<T> messageRepository,
            SocialExpressionFactory<T> expressionFactory)
            : base(name)
        {
            _messageRepository = messageRepository;
            _expressionFactory = expressionFactory;
        }

        /// <summary>
        /// Validates operation
        /// </summary>
        /// <param name="requestItem"> The request item. </param>
        /// <param name="token"> Security token. </param>
        /// <returns> True, if operation can be executed</returns>
        public override bool Validate(RequestItem requestItem, Gadgets.Security.ISecurityToken token)
        {
            // TODO validate whether user can perform this operation
            return true;
        }

        /// <summary>
        /// Process message operation
        /// </summary>
        /// <param name="requestItem"> The request item. </param>
        /// <param name="token"> Security token. </param>
        /// <returns> Async task</returns>
        public override Task<object> ProcessAsync(RequestItem requestItem, ISecurityToken token)
        {
            // TODO use dictionary?
            switch (requestItem.Operation)
            {
                case "get":
                    return ProcessGet(requestItem, token);
                case "send":
                    return ProcessSend(requestItem, token);
                case "create":
                    return ProcessCreate(requestItem, token);
                case "delete":
                    return this.ProcessDelete(requestItem, token);
                case "update":
                    return this.ProcessUpdate(requestItem, token);
                default:
                    return GetError(requestItem, string.Format("Operation '{0}' is not supported", requestItem.Operation));
            }
        }

        /// <summary>
        /// The Messages Service MAY support requests to retrieve Messages. If a Message-Collection-Id is not provided, 
        /// the response will contain a Collection of available Message-Collection-Ids for the given user.
        /// If a Message-Id is not provided, the response will contain a Collection of all messages in the specified message collection. 
        /// Requests and responses to retrieve Messages use the following values:
        /// REST-HTTP-Method       = "GET"
        /// REST-URI-Fragment      = "/messages/" User-Id [ "/" Message-Collection-Id [ "/" Message-Id ] ]
        /// REST-Query-Parameters  = ENCODE-REST-PARAMETERS(GetMessages-Request-Parameters)
        /// REST-Request-Payload   = null
        /// RPC-Method             = "messages.get"
        /// RPC-Request-Parameters = ENCODE-RPC-PARAMETERS(GetMessages-Request-Parameters)
        /// Return-Object          = Collection'Message-Collection-Id' / Message / Collection'Message'
        /// Message-Id             = Object-Id
        /// </summary>
        /// <param name="requestItem"> Request item. </param>
        /// <param name="token"> Security token. </param>
        /// <returns> Async task </returns>
        protected virtual Task<object> ProcessGet(RequestItem requestItem, ISecurityToken token)
        {
            // NOTE: need to get expression of IQueryable instance to build 'where' clause
            var source = _messageRepository.GetQueryable();

            var messageItem = requestItem.Entity as MessageItem<T>;
 
            Expression expression;
            if (string.IsNullOrEmpty(messageItem.MessageCollectionId))
            {
                expression = _expressionFactory.CreateCollectionListExpression(
                    requestItem.Params.UserId, messageItem, source.Expression);
            }
            else
            {
                if (string.IsNullOrEmpty(messageItem.MessageId))
                {
                    // TODO all messages of collection request
                    expression = _expressionFactory.CreateEntityListExpression(
                        requestItem.Params.UserId, messageItem.MessageCollectionId, messageItem, source.Expression);
                }
                else
                {
                    expression = _expressionFactory.CreateEntityExpression(
                        requestItem.Params.UserId,
                        messageItem.MessageCollectionId,
                        messageItem.MessageId,
                        messageItem,
                        source.Expression);
                }
            }

            // NOTE: see comment in people handler
            return _messageRepository.Select(expression).ContinueWith(
                t =>
                    {
                        var collection = t.Result;

                        // NOTE: it seems like collection result is always here
                        // object result = isCollectionResult
                        //                     ? this.GetCollectionResult(requestItem, collection) as object
                        //                     : this.GetRecordResult(requestItem, collection.SingleOrDefault()) as object;
                        object result = this.GetCollectionResult(requestItem, collection);

                        return AsyncHelper.GetEmptyTask(result);
                    }).Unwrap();
        }

        /// <summary>
        /// The Messages Service MAY support requests to create a message collection for the currently authenticated user and app. 
        /// The request MUST contain a name for the message collection. If successful, the container MUST return the ID 
        /// of the newly-created message collection. Requests and responses to create a message collection use the following values:
        /// REST-HTTP-Method        = "POST"
        /// REST-URI-Fragment       = "/messages/" User-Id "/@self"
        /// REST-Query-Parameters   = null 
        /// REST-Request-Payload    = Message-Collection-Name
        /// RPC-Method              = "messages.create"
        /// RPC-Request-Parameters  = ENCODE-RPC-PARAMETERS(CreateMessageCollection-Request-Parameters)
        /// Return-Object           = Message-Collection-Id
        /// Message-Collection-Name = String 
        /// </summary>
        /// <param name="requestItem"> Request item. </param>
        /// <param name="token"> Security token. </param>
        /// <returns> Async task</returns>
        protected virtual Task<object> ProcessCreate(RequestItem requestItem, ISecurityToken token)
        {
            var messageItem = requestItem.Entity as MessageItem<T>;
            var userId = requestItem.Params.UserId;
            
            // create new collection
            return _messageRepository.AddCollectionAsync(new EntityCollection<T>() 
                { 
                    UserId = userId, 
                    Type = messageItem.MessageCollectionId, 
                    Title = messageItem.Name 
                })
            .ContinueWith(t => (object)this.GetRecordResult(requestItem, new ResultEntry { Id = t.Result }));
        }

        /// <summary>
        /// The Messages Service MUST support requests to send a Message for the currently authenticated user and app. Placing a message in the outbox requests that the message be delivered to one or more recipients as specified in the message. Containers are free to filter or alter the message according to their own policies (such as security or rate limiting policies). If successful, the container MUST return the ID of the newly-created Message. Requests and responses to create a Messages use the following values:
        /// REST-HTTP-Method       = "POST"
        /// REST-URI-Fragment      = "/messages/" User-Id "/@self/@outbox"
        /// REST-Query-Parameters  = null 
        /// REST-Request-Payload   = Message
        /// RPC-Method             = "messages.send"
        /// RPC-Request-Parameters = ENCODE-RPC-PARAMETERS(SendMessage-Request-Parameters)
        /// Return-Object          = Message-Id
        /// Message-Id             = Object-Id
        /// </summary>
        /// <param name="requestItem"> Request item. </param>
        /// <param name="token"> Security token. </param>
        /// <returns> Async task</returns>
        protected virtual Task<object> ProcessSend(RequestItem requestItem, ISecurityToken token)
        {
            var messageItem = requestItem.Entity as MessageItem<T>;
            var userId = requestItem.Params.UserId;

            // add message to collection
            // NOTE messageCollectionId should be @outbox by spec (REST)
            return _messageRepository.AddEntityAsync(userId, messageItem.MessageCollectionId, messageItem.Message)
                .ContinueWith(t => (object)this.GetRecordResult(requestItem, t.Result));
        }

        /// <summary>
        /// The Messages Service MAY support requests to update a Message or message collection for the currently authenticated user and app, such as marking a message as 'read'. Requests and responses to update a Message use the following values:
        /// REST-HTTP-Method       = "PUT"
        /// REST-URI-Fragment      = "/messages/" User-Id "/@self/" Message-Collection-Id "/" Message-Id
        /// REST-Query-Parameters  = null 
        /// REST-Request-Payload   = Message
        /// RPC-Method             = "messages.update"
        /// RPC-Request-Parameters = ENCODE-RPC-PARAMETERS(UpdateMessage-Request-Parameters)
        /// Return-Object          = null
        /// Message-Id             = Object-Id
        /// </summary>
        /// Collection:
        /// The Messages Service MAY support requests to update a message collection for the currently authenticated user and app, such as renaming the message collection. Requests and responses to update a message collection use the following values:
        /// REST-HTTP-Method        = "PUT"
        /// REST-URI-Fragment       = "/messages/" User-Id "/@self/" Message-Collection-Id 
        /// REST-Query-Parameters   = null 
        /// REST-Request-Payload    = Message-Collection-Name
        /// RPC-Method              = "messages.update"
        /// RPC-Request-Parameters  = ENCODE-RPC-PARAMETERS(CreateMessageCollection-Request-Parameters)
        /// Return-Object           = null
        /// Message-Collection-Name = String 
        /// <param name="requestItem"> The request item. </param>
        /// <param name="token"> The token. </param>
        /// <returns> Async task</returns>
        private Task<object> ProcessUpdate(RequestItem requestItem, ISecurityToken token)
        {
            var messageItem = requestItem.Entity as MessageItem<T>;
            var userId = requestItem.Params.UserId;
            Task task;
            object result;
            if (!string.IsNullOrEmpty(messageItem.Name))
            {
                // update collection
                task = _messageRepository.UpdateCollectionAsync(new EntityCollection<T>()
                {
                    UserId = userId,
                    Type = messageItem.MessageCollectionId,
                    Title = messageItem.Name
                });
                result = (task as Task<string>).Result;
            }
            else
            {
                // update message
                task = _messageRepository.UpdateEntityAsync(userId, messageItem.MessageCollectionId, messageItem.Message);
                result = (task as Task<T>).Result;
            }

            return task.ContinueWith(t => (object)this.GetRecordResult(requestItem, result));
        }

        /// <summary>
        /// Delete a message or message collection
        /// The Messages Service MAY support requests to update a Message or message collection for the currently authenticated user and app, such as marking the message as 'read'. Requests and responses to update a Message use the following values:
        /// REST-HTTP-Method       = "DELETE"
        /// REST-URI-Fragment      = "/messages/" User-Id "/@self/" [ "/" Message-Collection-Id [ "/" Message-Id ] ]
        /// REST-Query-Parameters  = null 
        /// REST-Request-Payload   = null
        /// RPC-Method             = "messages.delete"
        /// RPC-Request-Parameters = ENCODE-RPC-PARAMETERS(DeleteMessage-Request-Parameters)
        /// Return-Object          = null
        /// Message-Id             = Object-Id
        /// </summary>
        /// <param name="requestItem"> The request item. </param>
        /// <param name="token"> The token. </param>
        /// <returns> Async task</returns>
        private Task<object> ProcessDelete(RequestItem requestItem, ISecurityToken token)
        {
            var messageItem = requestItem.Entity as MessageItem<T>;
            var userId = requestItem.Params.UserId;
            Task task;
            string result;
            if (string.IsNullOrEmpty(messageItem.MessageId))
            {
                task = _messageRepository.DeleteCollectionAsync(userId, messageItem.MessageCollectionId);
                result = (task as Task<string>).Result;
            }
            else
            {
                // TODO expensive operation
                if (messageItem.Message == null)
                {
                    messageItem.Message = (T)Activator.CreateInstance(typeof(T));
                }

                messageItem.Message.Id = messageItem.MessageId;
                task = _messageRepository.DeleteEntityAsync(userId, messageItem.MessageCollectionId, messageItem.Message);
                result = (task as Task<T>).Result.Id;
            }

            // NOTE container should return id here
            return task.ContinueWith(t => (object)this.GetRecordResult(requestItem, new ResultEntry { Id = result }));
        }
    }
}
