// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityStreamsController.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   RESTful service for activitystreams
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Web.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Catpic.Gadgets.Security;
    using Catpic.Social;
    using Catpic.Social.Activities;
    using Catpic.Social.Formatting;
    using Catpic.Web.Rules;

    /// <summary>
    /// RESTful service for activitystreams
    /// </summary>
    public class ActivityStreamsController : RestApiController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityStreamsController"/> class. 
        /// Creates an instance of activity streams api controller
        /// </summary>
        /// <param name="tokenFactory"> Security token factory.  </param>
        /// <param name="ruleChain"> The rule Chain. </param>
        /// <param name="services"> Social service handlers.  </param>
        public ActivityStreamsController(ISecurityTokenFactory tokenFactory, IRuleChain ruleChain, IEnumerable<SocialHandler> services)
            : base(tokenFactory, ruleChain, services)
        {
        }

        /// <summary>
        /// Gets activity streams
        /// </summary>
        /// <param name="requestItemParams"> The request Item Params. </param>
        /// <param name="activityItem"> Collection item </param>
        /// <returns> Async task </returns>
        public Task<object> GetAsync([FromUri] RequestParamsItem requestItemParams, [FromUri] ActivityItem<ActivityEntry> activityItem)
        {
            var requestItem = new RequestItem
            {
                Entity = activityItem,
                Id = string.Empty,
                Operation = "get",
                ServiceName = "activitystreams",
                Params = requestItemParams
            };
            return this.ProcessRequestItem(requestItem);
        }

        /// <summary>
        /// Post action
        /// </summary>
        /// <param name="requestItemParams"> Request parameter. </param>
        /// <param name="activity"> Activity entry. </param>
        /// <returns> Async task </returns>
        public Task<object> PostAsync([FromUri] RequestParamsItem requestItemParams, [FromBody] ActivityEntry activity)
        {
            var requestItem = new RequestItem
            {
                Entity = new ActivityItem<ActivityEntry>
                {
                    Activity = activity
                },
                Params = requestItemParams,
                Id = string.Empty,
                Operation = "create",
                ServiceName = "activityStreams"
            };

            return this.ProcessRequestItem(requestItem);
        }
    }
}
