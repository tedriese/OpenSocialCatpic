// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupHandler.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//  Containers MAY support the Groups Service. Individual operations are required or optional as indicated in the sections that follow. Containers that do support groups MUST use the following values to define the Groups Service:
//  XRDS-Type    = "http://ns.opensocial.org/2008/opensocial/groups"
//  Service-Name = "groups"
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Social.Groups
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Catpic.Gadgets.Security;
    using Catpic.Social.Formatting;
    using Catpic.Utils;

    /// <summary>
    /// Group social handler
    /// </summary>
    /// <typeparam name="T"> Group entity </typeparam>
    public class GroupHandler<T> : SocialHandler
        where T : IIdentityField
    {
        /// <summary>
        /// External group repository
        /// </summary>
        private readonly IRepository<T> _groupRepository;

        /// <summary>
        /// Group expression factory
        /// </summary>
        private readonly SocialExpressionFactory<T> _expressionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupHandler{T}"/> class.
        /// </summary>
        /// <param name="name"> Name of social service. </param>
        /// <param name="groupRepository"> Group repository. </param>
        /// <param name="expressionFactory"> Expression factory. </param>
        public GroupHandler(string name, IRepository<T> groupRepository, SocialExpressionFactory<T> expressionFactory)
            : base(name)
        {
            _groupRepository = groupRepository;
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
        /// Processes group request item. 
        /// </summary>
        /// <param name="requestItem">Request item. </param>
        /// <param name="token">Security token. </param>
        /// <returns>Group or Collection of Groups. </returns>
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
        /// 2.2.1 Get Groups
        /// Containers MUST support retrieving information about one or multiple groups in a single request. Requests and responses for retrieving groups use the following values:
        /// REST-HTTP-Method       = "GET"
        /// REST-URI-Fragment      = "/groups/" User-Id [ "/" Group-Id ]
        /// REST-Query-Parameters  = ENCODE-REST-PARAMETERS(GetGroups-Request-Parameters)
        /// REST-Request-Payload   = null
        /// RPC-Method             = "groups.get"
        /// RPC-Request-Parameters = ENCODE-RPC-PARAMETERS(GetGroups-Request-Parameters)
        /// Return-Object          = Group / Collection{Group}
        /// </summary>
        /// <param name="requestItem"> The request item. </param>
        /// <param name="token"> The token. </param>
        /// <returns> Async task</returns>
        protected virtual Task<object> ProcessGet(RequestItem requestItem, ISecurityToken token)
        {
            // NOTE: need to get expression of IQueryable instance to build 'where' clause
            var source = _groupRepository.GetQueryable();
            var item = requestItem.Entity as GroupItem<T>;

            Expression expression;

            bool isCollectionResult = true;
            
            // NOTE: standard supports only one collection which represents groups of current user, 
            // NOTE: However our implementation supports more collections..
            var collectionId = SocialConsts.GroupIdSelf;
            if (string.IsNullOrEmpty(item.GroupId))
            {
                expression = _expressionFactory.CreateEntityListExpression(
                    requestItem.Params.UserId, collectionId, item, source.Expression);
            }
            else
            {
                expression = _expressionFactory.CreateEntityExpression(
                    requestItem.Params.UserId, collectionId, item.GroupId, item, source.Expression);
                isCollectionResult = false;
            }

            return _groupRepository.Select(expression).ContinueWith(
                t =>
                    {
                        var collection = t.Result;
                        object result = isCollectionResult
                                            ? this.GetCollectionResult(requestItem, collection) as object
                                            : this.GetRecordResult(requestItem, collection.SingleOrDefault()) as object;

                        return AsyncHelper.GetEmptyTask(result);
                    }).Unwrap();
        }

        /// <summary>
        /// 2.2.2 Create a Group
        /// Containers MAY support the creation of groups.
        /// Requests and responses for creating a group use the following values:
        /// REST-HTTP-Method       = "POST"
        /// REST-URI-Fragment      = "/groups/" User-Id 
        /// REST-Query-Parameters  = null 
        /// REST-Request-Payload   = Group
        /// RPC-Method             = "groups.create"
        /// RPC-Request-Parameters = ENCODE-RPC-PARAMETERS(CreateGroup-Request-Parameters)
        /// Return-Object          = Group 
        /// </summary>
        /// <param name="requestItem"> The request item. </param>
        /// <param name="token"> The token. </param>
        /// <returns> Async task</returns>
        protected virtual Task<object> ProcessCreate(RequestItem requestItem, ISecurityToken token)
        {
            var item = requestItem.Entity as GroupItem<T>;
            var me = requestItem.Params.UserId;

            return this._groupRepository.AddEntityAsync(me, item.GroupId, item.Group)
                .ContinueWith(t => (object)this.GetRecordResult(requestItem, t.Result));
        }

        /// <summary>
        /// 2.2.3 Update a Group
        /// The update operation will update a group. Containers MAY support this request.
        /// REST-HTTP-Method       = "PUT"
        /// REST-URI-Fragment      = "/groups/" User-Id "/" Group-Id 
        /// REST-Query-Parameters  = null 
        /// REST-Request-Payload   = Group
        /// RPC-Method             = "groups.update"
        /// RPC-Request-Parameters = ENCODE-RPC-PARAMETERS(UpdateGroup-Request-Parameters)
        /// Return-Object          = Group
        /// </summary>
        /// <param name="requestItem">RequestItem object</param>
        /// <param name="token">Security token</param>
        /// <returns>Async task</returns>
        protected virtual Task<object> ProcessUpdate(RequestItem requestItem, ISecurityToken token)
        {
            var item = requestItem.Entity as GroupItem<T>;
            var me = requestItem.Params.UserId;

            return _groupRepository.UpdateEntityAsync(me, item.GroupId, item.Group)
                .ContinueWith(t => (object)this.GetRecordResult(requestItem, t.Result));
        }

        /// <summary>
        /// 2.2.4 Delete a Group
        /// Containers MAY support requests to remove a Group. Requests and responses to remove a Group use the following values:
        /// REST-HTTP-Method       = "DELETE"
        /// REST-URI-Fragment      = "/groups/" User-Id "/" Group-Id
        /// REST-Query-Parameters  = null
        /// REST-Request-Payload   = null
        /// RPC-Method             = "groups.delete"
        /// Return-Object          = null
        /// </summary>
        /// <param name="requestItem">RequestItem object</param>
        /// <param name="token">Security token</param>
        /// <returns>Async task</returns>
        protected virtual Task<object> ProcessDelete(RequestItem requestItem, ISecurityToken token)
        {
            var item = requestItem.Entity as GroupItem<T>;
            var me = requestItem.Params.UserId;

            // NOTE No need by spec
            if (item.Group == null)
            {
                item.Group = (T)Activator.CreateInstance(typeof(T));
            }

            item.Group.Id = item.GroupId;

            return _groupRepository.DeleteEntityAsync(me, item.GroupId, item.Group)
                .ContinueWith(t => (object)this.GetEmptyResult(requestItem));
        }
    }
}
