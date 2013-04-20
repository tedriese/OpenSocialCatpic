// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PeopleHandler.cs" company="Catpic Software">
//    Licensed under Apache License 2.0
// </copyright>
// <summary>
//   People handler which implements OpenSocial standard processing
//   XRDS-Type    = "http://ns.opensocial.org/2008/opensocial/people"
//   Service-Name = "people"
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Catpic.Social.People
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Catpic.Gadgets.Security;
    using Catpic.Social.Formatting;
    using Catpic.Utils;
    using Catpic.Utils.Diagnostic;

    /// <summary>
    /// People handler which implements OpenSocial standard processing
    /// XRDS-Type    = "http://ns.opensocial.org/2008/opensocial/people"
    /// Service-Name = "people"
    /// </summary>
    /// <typeparam name="T">Person entity</typeparam>
    public class PeopleHandler<T> : SocialHandler
        where T : IIdentityField
    {
        /// <summary>
        /// Trace category
        /// </summary>
        private const string TraceCategory = "social.people";

        /// <summary>
        /// Trace instance
        /// </summary>
        private static readonly ITrace Trace = TraceFactory.GetTrace();

        /// <summary>
        /// External repository of people
        /// </summary>
        private readonly IRepository<T> _repository;

        /// <summary>
        /// Expression factory for people service
        /// </summary>
        private readonly SocialExpressionFactory<T> _peopleExpressionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="PeopleHandler{T}"/> class. 
        /// Creates an instance of PeopleHandler 
        /// </summary>
        /// <param name="name"> Name of social service registration </param>
        /// <param name="repository"> External repository of people </param>
        /// <param name="peopleExpressionFactory"> Expression factory for people service </param>
        public PeopleHandler(string name, IRepository<T> repository, SocialExpressionFactory<T> peopleExpressionFactory)
            : base(name)
        {
            this._repository = repository;
            this._peopleExpressionFactory = peopleExpressionFactory;
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
        /// Processes people request item 
        /// </summary>
        /// <param name="requestItem">Request item</param>
        /// <param name="token">Security token</param>
        /// <returns>Async task</returns>
        public override Task<object> ProcessAsync(RequestItem requestItem, ISecurityToken token)
        {
            try
            {
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
                        return this.GetError(requestItem, string.Format("Operation '{0}' is not supported", requestItem.Operation));
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Operation '{0}' cannot be processed", requestItem.Operation);
                Trace.Error(TraceCategory, message, ex);
                return this.GetError(requestItem, message);
            }
        }

        /// <summary>
        /// 2.1.1.1 Get a Person
        /// Containers MUST support retrieving information about a person. Requests and responses for retrieving a 
        /// person use the following values:
        /// REST-HTTP-Method       = "GET"
        /// REST-URI-Fragment      = "/people/" User-Id "/@self"
        /// REST-Query-Parameters  = ENCODE-REST-PARAMETERS(GetPerson-Request-Parameters)
        /// REST-Request-Payload   = null
        /// RPC-Method             = "people.get"
        /// RPC-Request-Parameters = ENCODE-RPC-PARAMETERS(GetPerson-Request-Parameters)
        /// Return-Object          = Person
        /// 2.1.1.2 Get a list of People
        /// Containers MUST support retrieving information about multiple people in a single request. Requests and 
        /// responses for retrieving a list of people use the following values:
        /// REST-HTTP-Method       = "GET"
        /// REST-URI-Fragment      = "/people/" User-Id "/" Group-Id
        /// REST-Query-Parameters  = ENCODE-REST-PARAMETERS(GetPeople-Request-Parameters)
        /// REST-Request-Payload   = null
        /// RPC-Method             = "people.get"
        /// RPC-Request-Parameters = ENCODE-RPC-PARAMETERS(GetPeople-Request-Parameters)
        /// Return-Object          = Collection of Person 
        /// Note that the Group-Id can't be '@self' in these requests or only a single Person will be returned.
        /// </summary>
        /// <param name="requestItem">RequestItem object</param>
        /// <param name="token">Security token</param>
        /// <returns>Async task</returns>
        protected virtual Task<object> ProcessGet(RequestItem requestItem, ISecurityToken token)
        {
            var source = this._repository.GetQueryable();
            var item = requestItem.Entity as PersonItem<T>;

            Expression expression;
            bool isCollectionResult = true;
            if (string.IsNullOrEmpty(item.GroupId))
            {
                expression = _peopleExpressionFactory.CreateCollectionListExpression(
                    requestItem.Params.UserId, item, source.Expression);
            }
            else
            {
                // NOTE special case by protocol
                if (item.GroupId != SocialConsts.GroupIdSelf)
                {
                    expression = _peopleExpressionFactory.CreateEntityListExpression(
                        requestItem.Params.UserId, item.GroupId, item, source.Expression);
                }
                else
                {
                    var itemId = item.GroupId == SocialConsts.GroupIdSelf
                        ? requestItem.Params.UserId
                        : item.ItemId;
                       // : this.GetUserId(item.ItemId, token);

                    expression = _peopleExpressionFactory.CreateEntityExpression(
                        requestItem.Params.UserId,
                        item.GroupId,
                        itemId,
                        item,
                        source.Expression);
                    isCollectionResult = false;
                }
            }

            // NOTE: see comment in people handler
            return _repository.Select(expression).ContinueWith(
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
        /// 2.1.2.1 Create a relationship
        /// Containers MAY support request to create a new relationships between users. This is a generalization 
        /// of many use cases including invitation, and contact creation. Containers MAY require a dual opt-in process 
        /// before the friend record appears in the collection, and in this case SHOULD return a 202 Accepted response, 
        /// indicating that the request is 'in flight' and may or may not be ultimately successful.
        /// Requests and responses for creating a relationship use the following values:
        /// REST-HTTP-Method       = "POST"
        /// REST-URI-Fragment      = "/people/" User-Id "/" Group-Id
        /// REST-Query-Parameters  = null 
        /// REST-Request-Payload   = Person
        /// RPC-Method             = "people.create"
        /// RPC-Request-Parameters = ENCODE-RPC-PARAMETERS(CreateRelationship-Request-Parameters)
        /// Return-Object          = null
        /// </summary>
        /// <param name="requestItem">RequestItem object</param>
        /// <param name="token">Security token</param>
        /// <returns>Async task</returns>
        protected virtual Task<object> ProcessCreate(RequestItem requestItem, ISecurityToken token)
        {
            var item = requestItem.Entity as PersonItem<T>;
            var me = requestItem.Params.UserId;

            // NOTE empty result by spec
            return _repository.AddEntityAsync(me, item.GroupId, item.Person)
                .ContinueWith(t => (object)this.GetEmptyResult(requestItem));
        }

        /// <summary>
        /// 2.1.3 Update a Person
        /// Containers MAY support updating the properties of a Person object.
        /// If the request is successful, the container MUST return the updated Person object. 
        /// Requests and responses for updating the fields of a Person use the following values:
        /// REST-HTTP-Method       = "POST"
        /// REST-URI-Fragment      = "/people/" User-Id "/" Group-Id
        /// REST-Query-Parameters  = null 
        /// REST-Request-Payload   = Person
        /// RPC-Method             = "people.update"
        /// RPC-Request-Parameters = ENCODE-RPC-PARAMETERS(UpdatePerson-Request-Parameters)
        /// Return-Object          = Person
        /// </summary>
        /// <param name="requestItem">RequestItem object</param>
        /// <param name="token">Security token</param>
        /// <returns>Async task</returns>
        protected virtual Task<object> ProcessUpdate(RequestItem requestItem, ISecurityToken token)
        {
            return this.GetPersonActionTask(requestItem, token, _repository.UpdateEntityAsync);
        }

        /// <summary>
        /// 2.1.4 Delete a Person
        /// Containers MAY support requests to remove a Person. The default value for User-Id is "@me". Requests and responses to remove a Person use the following values:
        /// REST-HTTP-Method       = "DELETE"
        /// REST-URI-Fragment      = "/people/" User-Id "/@self"
        /// REST-Query-Parameters  = null
        /// REST-Request-Payload   = null
        /// RPC-Method             = "people.delete"
        /// RPC-Request-Parameters = "{" "userId"  ":" User-Id "}"
        /// Return-Object          = null
        /// </summary>
        /// <param name="requestItem">RequestItem object</param>
        /// <param name="token">Security token</param>
        /// <returns>Async task</returns>
        protected virtual Task<object> ProcessDelete(RequestItem requestItem, ISecurityToken token)
        {
            var item = requestItem.Entity as PersonItem<T>;
            var me = requestItem.Params.UserId;

            return _repository.DeleteEntityAsync(me, item.GroupId, item.Person)
                .ContinueWith(t => (object)GetEmptyResult(requestItem));
        }

        /// <summary>
        /// Builds typical CUD action
        /// </summary>
        /// <param name="requestItem"> The request item. </param>
        /// <param name="token"> The token. </param>
        /// <param name="action"> The action. </param>
        /// <typeparam name="T"> Person type</typeparam>
        /// <returns> Async task </returns>
        private Task<object> GetPersonActionTask(RequestItem requestItem, ISecurityToken token, Func<string, string, T, Task<T>> action)
        {
            var item = requestItem.Entity as PersonItem<T>;
            var me = requestItem.Params.UserId;

            return action(me, item.GroupId, item.Person)
                .ContinueWith(t => (object)this.GetRecordResult(requestItem, t.Result));
        }
    }
}
