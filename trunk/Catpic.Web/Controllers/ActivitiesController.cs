// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivitiesController.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   RESTful service for activities
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
    /// RESTful service for activities
    /// </summary>
    public class ActivitiesController : RestApiController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivitiesController"/> class. 
        /// </summary>
        /// <param name="tokenFactory"> Security token factory.  </param>
        /// <param name="ruleChain"> The rule Chain. </param>
        /// <param name="services"> Social service handlers.  </param>
        public ActivitiesController(ISecurityTokenFactory tokenFactory, IRuleChain ruleChain, IEnumerable<SocialHandler> services)
            : base(tokenFactory, ruleChain, services)
        {
        }

        /// <summary>
        /// Gets activities
        /// </summary>
        /// <param name="requestItemParams"> The request Item Params. </param>
        /// <param name="activityItem"> Activity item </param>
        /// <returns> Async task </returns>
        public Task<object> GetAsync([FromUri] RequestParamsItem requestItemParams, [FromUri] ActivityItem<Activity> activityItem)
        {
            var requestItem = new RequestItem
            {
                Entity = activityItem,
                Id = string.Empty,
                Operation = "get",
                ServiceName = "activities",
                Params = requestItemParams
            };
            return this.ProcessRequestItem(requestItem);
        }

        /// <summary>
        /// Post action
        /// </summary>
        /// <param name="requestItemParams"> The param. </param>
        /// <param name="activity"> The activity. </param>
        /// <returns> Async task </returns>
        public Task<object> PostAsync([FromUri] RequestParamsItem requestItemParams, [FromBody] Activity activity)
        {
            var requestItem = new RequestItem
            {
                Entity = new ActivityItem<Activity>
                {
                    Activity = activity
                },
                Params = requestItemParams,
                Id = string.Empty,
                Operation = "create",
                ServiceName = "activities"
            };

            return this.ProcessRequestItem(requestItem);
        }
    }
}
