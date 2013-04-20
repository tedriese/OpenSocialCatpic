// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestApiController.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Provides helpers methods to default social services via REST
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Catpic.Gadgets.Security;
    using Catpic.Social;
    using Catpic.Social.DTO;
    using Catpic.Utils;
    using Catpic.Utils.Diagnostic;
    using Catpic.Web.Rules;

    /// <summary>
    /// Provides helpers methods to default social services via REST
    /// </summary>
    public abstract class RestApiController : ApiController
    {
        /// <summary>
        /// Security token factory
        /// </summary>
        protected readonly ISecurityTokenFactory TokenFactory;

        /// <summary>
        /// Rules chain
        /// </summary>
        protected readonly IRuleChain RuleChain;

        /// <summary>
        /// Social service handlers list
        /// </summary>
        protected readonly IEnumerable<SocialHandler> Services;

        /// <summary>
        /// Trace category
        /// </summary>
        private const string TraceCategory = "social.rest";

        /// <summary>
        /// Trace instance
        /// </summary>
        private static readonly ITrace Trace = TraceFactory.GetTrace();

        /// <summary>
        /// Initializes a new instance of the <see cref="RestApiController"/> class. 
        /// </summary>
        /// <param name="tokenFactory"> Security token factory  </param>
        /// <param name="ruleChain"> The rule Chain. </param>
        /// <param name="services"> Social services list  </param>
        protected RestApiController(ISecurityTokenFactory tokenFactory, IRuleChain ruleChain, IEnumerable<SocialHandler> services)
        {
            TokenFactory = tokenFactory;
            this.RuleChain = ruleChain;
            Services = services;
        }

        /// <summary>
        /// Processes request item via  corresponding social handler
        /// </summary>
        /// <param name="requestItem"> Request item. </param>
        /// <returns> Async task. </returns>
        protected Task<object> ProcessRequestItem(RequestItem requestItem)
        {
            var service = Services.Single(s => s.Name == requestItem.ServiceName);
            IDictionary<string, object> requestProperties = Request != null ? Request.Properties : null;
            var token = TokenFactory.Create(User, requestProperties);
            var ruleContext = new RuleContext();
            if (!RuleChain.Validate(requestItem, token, ruleContext))
            {
                // TODO extract info from rule context
                string message = string.Format("Unable to validate: {0}", string.Join(", ", ruleContext.ValidationErrors));
                return AsyncHelper.GetEmptyTask(this.GetError(requestItem, message));
            }

            return service.ProcessAsync(requestItem, token);
        }

        /// <summary>
        /// Returns error object from requestItem
        /// </summary>
        /// <param name="requestItem"> Request item. </param>
        /// <param name="message"> Error message. </param>
        /// <returns> Error DTO. </returns>
        private object GetError(RequestItem requestItem, string message)
        {
            var error = new ErrorResult
            {
                Id = requestItem != null ? requestItem.Id : string.Empty,
                Error = new ErrorDetails { Code = 500, Message = message }
            };
            Trace.Error(TraceCategory, error.Error.Message, null);
            return error;
        }
    }
}
