// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityHandler.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Provides logic for activity handling
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Social.Activities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Catpic.Gadgets.Security;
    using Catpic.Social.Formatting;
    using Catpic.Social.People;
    using Catpic.Utils;

    /// <summary>
    /// Containers MUST support the Activity Streams Service. Individual operations are required or optional as indicated in the sections that follow. Containers MUST use the following values to define the Activity Streams Service:
    /// XRDS-Type    = "http://ns.opensocial.org/2008/opensocial/activitystreams"
    /// Service-Name = "activitystreams"
    /// </summary>
    /// <typeparam name="T">Activity type</typeparam>
    public class ActivityHandler<T> : SocialHandler where T: IIdentityField
    {
        /// <summary>
        /// External activity repository
        /// </summary>
        private readonly IRepository<T> _activityRepository;

        /// <summary>
        /// Activity expression factory
        /// </summary>
        private readonly SocialExpressionFactory<T> _expressionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityHandler{T}"/> class.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <param name="activityRepository"> The activity repository. </param>
        /// <param name="expressionFactory"> The expression factory. </param>
        public ActivityHandler(
            string name,
            IRepository<T> activityRepository,
            SocialExpressionFactory<T> expressionFactory)
            : base(name)
        {
            _activityRepository = activityRepository;
            _expressionFactory = expressionFactory;
        }

        /// <summary>
        /// True, if request can be handled
        /// </summary>
        /// <param name="requestItem">Request item</param>
        /// <param name="token">Security token</param>
        /// <returns>true, if request can be processed</returns>
        public override bool Validate(RequestItem requestItem, ISecurityToken token)
        {
            return true;
        }

        /// <summary>
        /// Processes activity request item 
        /// </summary>
        /// <param name="requestItem">Request item</param>
        /// <param name="token">Security token</param>
        /// <returns>Async task</returns>
        public override Task<object> ProcessAsync(RequestItem requestItem, ISecurityToken token)
        {           
            // explore requestItem.operation
            switch (requestItem.Operation)
            {
                case "get":
                    return this.ProcessGet(requestItem, token);
                case "create":
                    return this.ProcessCreate(requestItem, token);
                case "delete":
                    return this.ProcessDelete(requestItem, token);
                case "update":
                    return this.ProcessUpdate(requestItem, token);
                default:
                    return GetError(requestItem, string.Format("Operation '{0}' is not supported", requestItem.Operation));
            }
        }

        /// <summary>
        /// 2.3.1 Get Activity Streams
        /// The Get method supports multiple queries, including requests for one or more Activity Stream entries and what fields are available.
        /// Containers MUST support requests to the Activity Stream service. Requests and responses for retrieving Activity Stream entries use the following values:
        /// REST-HTTP-Method       = "GET"
        /// REST-URI-Fragment      = "/activitystreams/" User-Id "/" Group-Id [ "/" App-Id [ "/" (Activity-Id / Array of Activity-Ids) ] ] 
        /// REST-Query-Parameters  = ENCODE-REST-PARAMETERS(GetActivityStreams-Request-Parameters)
        /// REST-Request-Payload   = null
        /// RPC-Method             = "activitystreams.get"
        /// RPC-Request-Parameters = ENCODE-RPC-PARAMETERS(GetActivityStreams-Request-Parameters)
        /// Return-Object          = ActivityEntry  / Collection of ActivityEntries
        /// Activity-Id            = Object-Id
        /// </summary>
        /// <param name="requestItem">Request item</param>
        /// <param name="token">Security token</param>
        /// <returns>Async task</returns>
        protected virtual Task<object> ProcessGet(RequestItem requestItem, ISecurityToken token)
        {
            var source = this._activityRepository.GetQueryable();

            var activityItem = requestItem.Entity as ActivityItem<T>;

            Expression expression;
            if (string.IsNullOrEmpty(activityItem.GroupId))
            {
                expression = _expressionFactory.CreateCollectionListExpression(
                    requestItem.Params.UserId, activityItem, source.Expression);
            }
            else
            {
                if (string.IsNullOrEmpty(activityItem.ItemId))
                {
                    // TODO all messages of collection request
                    expression = _expressionFactory.CreateEntityListExpression(
                        requestItem.Params.UserId, activityItem.GroupId, activityItem, source.Expression);
                }
                else
                {
                    expression = _expressionFactory.CreateEntityExpression(
                        requestItem.Params.UserId,
                        activityItem.GroupId,
                        activityItem.ItemId,
                        activityItem,
                        source.Expression);
                }
            }

            return _activityRepository.Select(expression).ContinueWith(
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
        /// 2.3.2 Create an ActivityEntry
        /// Containers MUST support creating an ActivityEntry associated with the currently authenticated app. If successful, the container MUST return the ID of the newly-created ActivityEntry. For details on uploading content associated with an ActivityEntry, see the Content Upload section of [Core-API-Server].
        /// Requests and responses to create an ActivityEntry use the following values:
        /// REST-HTTP-Method       = "POST"
        /// REST-URI-Fragment      = "/activitystreams/" User-Id "/@self"
        /// REST-Query-Parameters  = null
        /// REST-Request-Payload   = ActivityEntry
        /// RPC-Method             = "activitystreams.create"
        /// RPC-Request-Parameters = ENCODE-RPC-PARAMETERS(CreateActivityEntries-Request-Parameters)
        /// Return-Object          = ActivityEntry
        /// </summary>
        /// <param name="requestItem">Request item</param>
        /// <param name="token">Security token</param>
        /// <returns>Async task</returns>
        protected virtual Task<object> ProcessCreate(RequestItem requestItem, ISecurityToken token)
        {
            var item = requestItem.Entity as ActivityItem<T>;
            var me = requestItem.Params.UserId;

            return this._activityRepository.AddEntityAsync(me, item.GroupId, item.Activity)
                .ContinueWith(t => (object)this.GetRecordResult(requestItem, t.Result));
        }

        /// <summary>
        /// 2.3.3 Update an ActivityEntry
        /// Containers MAY support updating an ActivityEntry associated with the currently authenticated app. For details on uploading content associated with an Activity Stream, see the Content Upload section of [Core-API-Server].
        /// Requests and responses to update an ActivityEntry use the following values:
        /// REST-HTTP-Method       = "PUT"
        /// REST-URI-Fragment      = "/activitystreams/" User-Id "/@self"
        /// REST-Query-Parameters  = null
        /// REST-Request-Payload   = ActivityEntry
        /// RPC-Method             = "activitystreams.update"
        /// RPC-Request-Parameters = ENCODE-RPC-PARAMETERS(UpdateActivityStreams-Request-Parameters)
        /// Return-Object          = ActivityEntry
        /// </summary>
        /// <param name="requestItem">RequestItem object</param>
        /// <param name="token">Security token</param>
        /// <returns>Person item</returns>
        protected virtual Task<object> ProcessUpdate(RequestItem requestItem, ISecurityToken token)
        {
            var item = requestItem.Entity as ActivityItem<T>;
            var me = requestItem.Params.UserId;

            return _activityRepository.UpdateEntityAsync(me, item.GroupId, item.Activity)
                .ContinueWith(t => (object)this.GetRecordResult(requestItem, t.Result));
        }

        /// <summary>
        /// 2.3.4 Delete an ActivityEntry
        /// Containers MAY support deleting an ActivityEntry associated with the currently authenticated app. Requests and responses to update an ActivityEntry use the following values:
        /// REST-HTTP-Method       = "DELETE"
        /// REST-URI-Fragment      = "/activitystreams/" User-Id "/@self"
        /// REST-Query-Parameters  = null
        /// REST-Request-Payload   = null
        /// RPC-Method             = "activitystreams.delete"
        /// RPC-Request-Parameters = ENCODE-RPC-PARAMETERS(DeleteActivityEntries-Request-Parameters)
        /// Return-Object        
        /// </summary>
        /// <param name="requestItem">RequestItem object</param>
        /// <param name="token">Security token</param>
        /// <returns>Async task</returns>
        protected virtual Task<object> ProcessDelete(RequestItem requestItem, ISecurityToken token)
        {
            var item = requestItem.Entity as ActivityItem<T>;
            var me = requestItem.Params.UserId;

            // TODO expensive operation
            if (item.Activity == null)
            {
                item.Activity = (T)Activator.CreateInstance(typeof(T));
            }

            item.Activity.Id = item.ItemId;
            return _activityRepository.DeleteEntityAsync(me, item.GroupId, item.Activity)
                .ContinueWith(t => (object)this.GetEmptyResult(requestItem));
        }
    }
}
