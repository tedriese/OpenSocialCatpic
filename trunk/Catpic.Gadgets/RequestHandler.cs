// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestHandler.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Facade for gadget-specific processing
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets
{
    using System;
    using System.Threading.Tasks;

    using Catpic.Gadgets.Containers;
    using Catpic.Gadgets.Proxies;
    using Catpic.Gadgets.Rendering.Container;
    using Catpic.Gadgets.Rendering.Gadget;
    using Catpic.Gadgets.Security;
    using Catpic.Utils;
    using Catpic.Utils.Diagnostic;

    /// <summary>
    /// Facade for gadget-specific processing
    /// </summary>
    public class RequestHandler : IRequestHandler
    {
        /// <summary>
        /// Trace category.
        /// </summary>
        private const string TraceCategory = "gadget.handler";

        /// <summary>
        /// Trace instance.
        /// </summary>
        private static readonly ITrace Trace = TraceFactory.GetTrace();

        /// <summary>
        /// Container provider which resolves containers
        /// </summary>
        private readonly IContainerProvider _containerProvider;
        
        /// <summary>
        /// Gadget definition factory which produces gadget definition
        /// </summary>
        private readonly GadgetDefinitionFactory _factory;

        /// <summary>
        /// Container rendering pipeline which renders environment scripts
        /// </summary>
        private readonly IContainerRenderPipeline _containerRenderPipeline;

        /// <summary>
        /// Gadget rendering pipeline which renders gadgets
        /// </summary>
        private readonly GadgetRenderPipeline _gadgetRenderPipeline;

        /// <summary>
        /// Security request handler which is used by oauth functionality
        /// </summary>
        private readonly ISecurityRequestHandler _securityRequestHandler;

        /// <summary>
        /// Proxy for proxy requests
        /// </summary>
        private readonly IRequestProxy _requestProxy;

        /// <summary>
        /// Concat proxy
        /// </summary>
        private readonly IConcatProxy _concatProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestHandler"/> class.
        /// </summary>
        /// <param name="containerProvider">
        /// Container provider.
        /// </param>
        /// <param name="containerRenderPipeline">
        /// Container render pipeline.
        /// </param>
        /// <param name="gadgetRenderPipeline">
        /// Gadget render pipeline.
        /// </param>
        /// <param name="factory">
        /// Gadget definition factory.
        /// </param>
        /// <param name="securityRequestHandler">
        /// Security request handler.
        /// </param>
        /// <param name="requestProxy">
        /// Request proxy.
        /// </param>
        /// <param name="concatProxy">
        /// Concat proxy.
        /// </param>
        public RequestHandler(
            IContainerProvider containerProvider,
            IContainerRenderPipeline containerRenderPipeline, 
            GadgetRenderPipeline gadgetRenderPipeline, 
            GadgetDefinitionFactory factory,
            ISecurityRequestHandler securityRequestHandler,
            IRequestProxy requestProxy,
            IConcatProxy concatProxy)
        {
            this._containerProvider = containerProvider;
            this._containerRenderPipeline = containerRenderPipeline;
            this._gadgetRenderPipeline = gadgetRenderPipeline;
            this._factory = factory;

            this._securityRequestHandler = securityRequestHandler;
            this._requestProxy = requestProxy;
            this._concatProxy = concatProxy;
        }

        #region container call

        /// <summary>
        /// Processes container environment call
        /// </summary>
        /// <param name="context"> Container context. </param>
        /// <returns>Async task</returns>
        public Task ContainerAsync(ContainerContext context)
        {
            return this.GetContainerTask(context);
        }
         
        #endregion

        #region metadata call

        /// <summary>
        /// Processes metadata request
        /// </summary>
        /// <param name="context"> Container context. </param>
        /// <returns>Async task</returns>
        public Task MetadataAsync(ContainerContext context)
        {
            return this.GetContainerTask(context);
        }

        #endregion

        #region gadget creation

        /// <summary>
        /// Creates gadget
        /// </summary>
        /// <param name="context"> gadget context. </param>
        /// <returns>Async task</returns>
        public Task CreateGadgetAsync(GadgetContext context)
        {
            // load gadget xml from remote/local resource
            if (!context.IsCacheEnabled || this._factory.Get(context.Uri) == null)
            {
                Trace.Debug(TraceCategory, string.Format("create: try to get {0}", context.Uri));

                return
                    RemoteFetchHelper.GetFetchDataTask(context.Uri, "GET", null, TaskCreationOptions.None).ContinueWith(
                        t =>
                            {
                                Trace.Debug(TraceCategory, string.Format("create: start parsing response {0}", context.Uri));
                                try
                                {
                                    var response = t.Result;
                                    var gadgetDefinition = _factory.Create(context.Uri, response);
                                    Trace.Debug(TraceCategory, string.Format("create: end parsing response {0}", context.Uri));
                                    return GetGadgetRenderTask(new Gadget(gadgetDefinition, context));
                                }
                                catch (Exception ex)
                                {
                                    Trace.Error(TraceCategory, string.Format("create: unable to process {0}", context.Uri), ex);
                                    throw;
                                }
                            },
                        TaskContinuationOptions.ExecuteSynchronously).Unwrap();
            }

            // load gadget definition from cache
            Trace.Debug(TraceCategory, string.Format("create: cache hit {0}", context.Uri));
            return this.GetGadgetRenderTask(new Gadget(this._factory.Get(context.Uri), context));
        }

        #endregion

        #region gadget proxy

        /// <summary>
        /// Processes make request call
        /// </summary>
        /// <param name="context"> Proxy context. </param>
        /// <returns> Async task</returns>
        public Task MakeRequestAsync(ProxyContext context)
        {
            return this._securityRequestHandler.ProcessRequest(context, this._requestProxy.MakeRequestAsync);
        }

        /// <summary>
        /// Processes proxy request
        /// </summary>
        /// <param name="context"> proxy context. </param>
        /// <returns> Async task</returns>
        public Task ProxyRequestAsync(ProxyContext context)
        {
            return this._securityRequestHandler.ProcessRequest(context, this._requestProxy.ProxyRequestAsync);
        }

        #endregion

        #region gadget concat

        /// <summary>
        /// Processes concat request which concatenates client-resources
        /// </summary>
        /// <param name="context"> Gadget context. </param>
        /// <returns> Async task </returns>
        public Task ConcatScriptAsync(GadgetContext context)
        {
            return this._concatProxy.RenderScriptAsync(context);
        }

        #endregion

        #region security callback

        /// <summary>
        /// Process security callback (used by oauth functionality)
        /// </summary>
        ///  <param name="context"> Proxy context. </param>
        /// <returns> Async task </returns>
        public Task SecurityCallbackAsync(ProxyContext context)
        {
            return this._securityRequestHandler.ProcessCallback(context);
        }

        #endregion

        #region private members

        /// <summary>
        /// Return task which process request through rendering pipeline
        /// </summary>
        /// <param name="context"> Container context</param>
        /// <returns>Async task</returns>
        private Task GetContainerTask(ContainerContext context)
        {
            var gadgetsContainer = this._containerProvider.GetContainer(context.Name);
            return this._containerRenderPipeline.RenderAsync(gadgetsContainer, context, context.Http.Response.Output);
        }

        /// <summary>
        /// Returns gadget rendering task
        /// </summary>
        /// <param name="gadget"> Gadget instance. </param>
        /// <returns> Async task</returns>
        private Task GetGadgetRenderTask(Gadget gadget)
        {
            // TODO check whether gadget is null
            var container = this._containerProvider.GetContainer(gadget.Context.ContainerName);
            return this._gadgetRenderPipeline.RenderAsync(container, gadget, gadget.Context.Http.Response.Output);
        }

        #endregion
    }
}
