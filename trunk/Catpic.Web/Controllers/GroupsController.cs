// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupsController.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   RESTful service for groups
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Web.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Catpic.Gadgets.Security;
    using Catpic.Social;
    using Catpic.Social.Formatting;
    using Catpic.Social.Groups;
    using Catpic.Web.Rules;

    /// <summary>
    /// RESTful service for groups
    /// </summary>
    public class GroupsController : RestApiController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupsController"/> class. 
        /// </summary>
        /// <param name="tokenFactory"> Security token factory.  </param>
        /// <param name="ruleChain"> The rule Chain. </param>
        /// <param name="services"> Social service handlers.  </param>
        public GroupsController(ISecurityTokenFactory tokenFactory, IRuleChain ruleChain, IEnumerable<SocialHandler> services)
            : base(tokenFactory, ruleChain, services)
        {
        }

        /// <summary>
        /// Gets groups
        /// </summary>
        /// <param name="requestItemParams"> The request Item Params. </param>
        /// <param name="activityItem"> Activity item </param>
        /// <returns> Async task </returns>
        public Task<object> GetAsync([FromUri] RequestParamsItem requestItemParams, [FromUri] GroupItem<Group> activityItem)
        {
            var requestItem = new RequestItem
            {
                Entity = activityItem,
                Id = string.Empty,
                Operation = "get",
                ServiceName = "groups",
                Params = requestItemParams
            };
            return this.ProcessRequestItem(requestItem);
        }

        /// <summary>
        /// Post action
        /// </summary>
        /// <param name="requestItemParams"> The param. </param>
        /// <param name="group"> The group. </param>
        /// <returns> Async task </returns>
        public Task<object> PostAsync([FromUri] RequestParamsItem requestItemParams, [FromBody] Group group)
        {
            var requestItem = new RequestItem
            {
                Entity = new GroupItem<Group>
                {
                    Group = group
                },
                Params = requestItemParams,
                Id = string.Empty,
                Operation = "create",
                ServiceName = "groups"
            };

            return this.ProcessRequestItem(requestItem);
        }
    }
}
