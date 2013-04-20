// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RpcController.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Handles RPC calls
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Catpic.Gadgets.Security;
    using Catpic.Social;
    using Catpic.Social.DTO;
    using Catpic.Utils;
    using Catpic.Utils.Diagnostic;
    using Catpic.Web.Rules;

    /// <summary>
    /// Handles RPC calls
    /// </summary>
    public class RpcController : ApiController
    {
        /// <summary>
        /// Trace category
        /// </summary>
        private const string TraceCategory = "social.rpc";
        
        /// <summary>
        /// Trace instance
        /// </summary>
        private static readonly ITrace Trace = TraceFactory.GetTrace();

        /// <summary>
        /// Security token factory
        /// </summary>
        private readonly ISecurityTokenFactory _tokenFactory;

        /// <summary>
        /// Rules chain
        /// </summary>
        private readonly IRuleChain _ruleChain;

        /// <summary>
        /// Social service handlers
        /// </summary>
        private readonly IEnumerable<SocialHandler> _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="RpcController"/> class.
        /// </summary>
        /// <param name="tokenFactory"> Security token factory.  </param>
        /// <param name="ruleChain"> The rule Chain. </param>
        /// <param name="services"> Social service handlers  </param>
        public RpcController(ISecurityTokenFactory tokenFactory, IRuleChain ruleChain, IEnumerable<SocialHandler> services)
        {
            _tokenFactory = tokenFactory;
            _ruleChain = ruleChain;
            _services = services;
        }

        /// <summary>
        /// Handles all POST requests and processes them
        /// </summary>
        /// <param name="requestItems">Request item</param>
        /// <returns>Async task</returns>
        [HttpPost]
        public Task<IEnumerable<object>> PostAsync(IEnumerable<RequestItem> requestItems)
        {
            if (requestItems == null)
            {
                return AsyncHelper.GetEmptyTask((new List<object> { GetError(null, "Unable to deserialize") }).AsEnumerable());
            }

            IDictionary<string, object> requestProperties = Request != null ? Request.Properties : null;

            var token = _tokenFactory.Create(User, requestProperties);
            var cts = new CancellationTokenSource();
            var tf = new TaskFactory(
                cts.Token,
                TaskCreationOptions.AttachedToParent,
                TaskContinuationOptions.ExecuteSynchronously,
                                     TaskScheduler.Default);
            var tasks = new List<Task<object>>();
            
            for (int i = 0; i < tasks.Count; i++)
            {
                tasks[i].ContinueWith(t => cts.Cancel(), TaskContinuationOptions.OnlyOnFaulted);
            }

            // build task list
            foreach (var requestItem in requestItems)
            {
                Task<object> task;
                var r = requestItem;
                if (r != null && _services.Any(s => s.Name == r.ServiceName))
                {
                    Trace.Debug(TraceCategory, string.Format("process {0}.{1}", r.ServiceName, r.Operation));

                    var ruleContext = new RuleContext();
                    if (!this._ruleChain.Validate(requestItem, token, ruleContext))
                    {
                        string message = string.Format("Unable to validate {0}: {1}", r.Id, string.Join(", ", ruleContext.ValidationErrors));
                        task = AsyncHelper.GetEmptyTask(this.GetError(r, message));
                    }
                    else
                    {
                        var handler = _services.Single(s => s.Name == r.ServiceName);
                        task = handler.ProcessAsync(requestItem, token);
                    }
                }
                else
                {
                    string message = r != null
                                         ? string.Format("'{0}' service isn't supported", r.ServiceName)
                                         : "Unable to deserialize";
                    task = AsyncHelper.GetEmptyTask(this.GetError(r, message));
                }

                tasks.Add(task);
            }

            return tf.ContinueWhenAll(tasks.ToArray(), completedTasks => completedTasks.Select(t => t.Result));
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
