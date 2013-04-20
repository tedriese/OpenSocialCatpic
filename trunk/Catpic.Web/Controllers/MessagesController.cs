// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagesController.cs" company="Catpic Software">
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
    using Catpic.Social.Formatting;
    using Catpic.Social.Messages;
    using Catpic.Web.Rules;

    /// <summary>
    /// RESTful service for messages
    /// </summary>
    public class MessagesController : RestApiController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagesController"/> class. 
        /// </summary>
        /// <param name="tokenFactory"> Security token factory.  </param>
        /// <param name="ruleChain"> The rule Chain. </param>
        /// <param name="services"> Social service handlers.  </param>
        public MessagesController(ISecurityTokenFactory tokenFactory, IRuleChain ruleChain, IEnumerable<SocialHandler> services)
            : base(tokenFactory, ruleChain, services)
        {
        }

        /// <summary>
        /// Gets messages or message collection list
        /// </summary>
        /// <param name="requestItemParams"> The request Item Params. </param>
        /// <param name="collection"> Collection items </param>
        /// <returns> Async task </returns>
        public Task<object> GetAsync([FromUri] RequestParamsItem requestItemParams, [FromUri] MessageItem<Message> collection)
        {
            var requestItem = new RequestItem
            {
                Entity = collection,
                Id = string.Empty,
                Operation = "get",
                ServiceName = "messages",
                Params = requestItemParams
            };
            return this.ProcessRequestItem(requestItem);
        }

        /// <summary>
        /// Post action
        /// </summary>
        /// <param name="requestItemParams"> The param. </param>
        /// <param name="message"> The message. </param>
        /// <returns> Async task </returns>
        public Task<object> PostAsync([FromUri] RequestParamsItem requestItemParams, [FromBody] Message message)
        {
            var requestItem = new RequestItem
            {
                Entity = new MessageItem<Message>
                {
                    Message = message,
                },
                Params = requestItemParams,
                Id = string.Empty,
                Operation = "create",
                ServiceName = "messages"
            };

            return this.ProcessRequestItem(requestItem);
        }
    }
}
