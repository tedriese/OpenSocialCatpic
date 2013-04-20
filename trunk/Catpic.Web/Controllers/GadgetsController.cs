// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GadgetsController.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Handles client's side calls
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Web.Controllers
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Catpic.Gadgets;
    using Catpic.Gadgets.Proxies;
    using Catpic.Social.DTO;
    using Catpic.Utils;
    using Catpic.Utils.Diagnostic;

    using Newtonsoft.Json;

    /// <summary>
    /// Handles client's side calls TODO create it as WebAPI controller?
    /// </summary>
    public class GadgetsController : AsyncController
    {
        /// <summary>
        /// Trace category
        /// </summary>
        private const string TraceCategory = "controller.gadgets";

        /// <summary>
        /// Trace instance
        /// </summary>
        private static readonly ITrace Trace = TraceFactory.GetTrace();

        /// <summary>
        /// Request hadnler
        /// </summary>
        private readonly IRequestHandler _requestHandler;

        /// <summary>
        /// Context factory
        /// </summary>
        private readonly IContextFactory _contextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="GadgetsController"/> class.
        /// </summary>
        /// <param name="requestHandler"> Request handler. </param>
        /// <param name="contextFactory"> Context factory. </param>
        public GadgetsController(IRequestHandler requestHandler, IContextFactory contextFactory)
        {
            _requestHandler = requestHandler;
            _contextFactory = contextFactory;
        }

        /// <summary>
        /// Begin action
        /// </summary>
        public void EnvironmentAsync()
        {
            Trace.Debug(TraceCategory, "container.begin");
            ContainerContext context = _contextFactory.CreateContainerContext(HttpContext);
            AsyncManager.OutstandingOperations.Increment();
            _requestHandler.ContainerAsync(context)
                .ContinueWith(t =>
                {
                    try
                    {
                        // TODO explore status of task
                    }
                    finally
                    {
                        AsyncManager.OutstandingOperations.Decrement();
                    }
                });
        }

        /// <summary>
        /// Action completed
        /// </summary>
        /// <returns> Action result</returns>
        public ActionResult EnvironmentCompleted()
        {
            Trace.Debug(TraceCategory, "container.end");
            Response.ContentType = "application/x-javascript";
            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }

        /// <summary>
        /// Begin action
        /// </summary>
        public void MetadataAsync()
        {
            Trace.Debug(TraceCategory, "metadata.begin");
            ContainerContext context = _contextFactory.CreateContainerContext(HttpContext);
            AsyncManager.OutstandingOperations.Increment();
            _requestHandler.MetadataAsync(context)
                .ContinueWith(t =>
                {
                    try
                    {
                        // TODO explore status of task
                    }
                    finally
                    {
                        AsyncManager.OutstandingOperations.Decrement();
                    }
                });
        }

        /// <summary>
        /// Action completed
        /// </summary>
        /// <returns> Action result</returns>
        public ActionResult MetadataCompleted()
        {
            Trace.Debug(TraceCategory, "metadata.end");
            Response.ContentType = "application/x-javascript";
            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }

        #region Create gadget

        /// <summary>
        /// Begin action
        /// </summary>
        public void IfrAsync()
        {
            GadgetContext context = _contextFactory.CreateGadgetContext(HttpContext);
            Trace.Debug(TraceCategory, string.Format("create.begin: {0}", context.Uri));
            AsyncManager.OutstandingOperations.Increment();
            _requestHandler.CreateGadgetAsync(context)
                .ContinueWith(t =>
                {
                    try
                    {
                        ErrorResult error = null;
                        
                        // explore status of task
                        if (t.Status == TaskStatus.Faulted)
                        {
                            error = GetErrorResult(500, string.Format("Unable to create gadget {0}", context.Uri));
                        }

                        AsyncManager.Parameters["error"] = error;
                    }
                    finally
                    {
                        AsyncManager.OutstandingOperations.Decrement();
                    }
                });
        }

        /// <summary>
        /// Action completed
        /// </summary>
        /// <param name="error"> Error DTO. </param>
        /// <returns>
        /// Action result
        /// </returns>
        public ActionResult IfrCompleted(ErrorResult error)
        {
            Trace.Debug(TraceCategory, "create.end");
            if (error != null)
            {
                // NOTE: built-in json serilizer doesn't account for DataMember attributes
                Response.ContentType = @"application/json";
                return this.Content(JsonConvert.SerializeObject(error)); 
            }

            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }

        #endregion

        #region Make request

        /// <summary>
        /// Begin action
        /// </summary>
        public void MakeRequestAsync()
        {
            ProxyContext context = _contextFactory.CreateProxyContext(HttpContext);
            Trace.Debug(TraceCategory, string.Format("makeRequest.begin: {0}", context.Http.Request.Url));
            AsyncManager.OutstandingOperations.Increment();
            _requestHandler.MakeRequestAsync(context)
                .ContinueWith(t =>
                {
                    try
                    {
                        // TODO explore status of task
                    }
                    finally
                    {
                        AsyncManager.OutstandingOperations.Decrement();
                    }
                });
        }

        /// <summary>
        /// Action completed
        /// </summary>
        /// <returns> Action result</returns>
        public ActionResult MakeRequestCompleted()
        {
            Trace.Debug(TraceCategory, string.Format("makeRequest.end"));

            Response.AddHeader("Content-Disposition", "attachment;filename=p.txt");
            Response.ContentType = @"application/json";
            
            // NOTE expecting of content writing to Response directly
            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }

        #endregion

        #region Proxy request

        /// <summary>
        /// Begin action
        /// </summary>
        public void ProxyAsync()
        {
            AsyncManager.OutstandingOperations.Increment();
            ProxyContext context = _contextFactory.CreateProxyContext(HttpContext);
            Trace.Debug(TraceCategory, string.Format("proxyRequest.begin: {0}", context.Http.Request.Url));
            _requestHandler.ProxyRequestAsync(context)
                .ContinueWith(t =>
                {
                    try
                    {
                        // TODO explore status of task
                    }
                    catch (Exception ex)
                    {
                        Trace.Error(TraceCategory, "unable to proxy request", ex);
                    }
                    finally
                    {
                        AsyncManager.OutstandingOperations.Decrement();
                    }
                });
        }

        /// <summary>
        /// Action completed
        /// </summary>
        /// <returns> Action result</returns>
        public ActionResult ProxyCompleted()
        {
            Trace.Debug(TraceCategory, string.Format("proxyRequest.end"));
            
            // NOTE expecting of content writing to Response directly
            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }

        #endregion

        #region Concat request

        /// <summary>
        /// Begin action
        /// </summary>
        public void ConcatAsync()
        {
            Trace.Debug(TraceCategory, "concat.begin");
            
            // css can be concatted too
            var context = _contextFactory.CreateGadgetContext(HttpContext);
            
            // TODO separate logic of processing scripts and css
            AsyncManager.OutstandingOperations.Increment();
            _requestHandler.ConcatScriptAsync(context).ContinueWith(
                t =>
                    {
                        try
                        {
                            // TODO explore status of task
                        }
                        finally
                        {
                            AsyncManager.OutstandingOperations.Decrement();
                        }
                    });
        }

        /// <summary>
        /// Action completed
        /// </summary>
        /// <returns> Action result</returns>
        public ActionResult ConcatCompleted()
        {
            // NOTE expecting of content writing to Response directly
            Trace.Debug(TraceCategory, "concat.end");
            Response.AddHeader("Content-Disposition", "attachment;filename=p.txt");
            Response.ContentType = @"application/x-javascript";
            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }

        #endregion

        #region OAuth callback

        /// <summary>
        /// Begin action
        /// </summary>
        public void Oauth_CallbackAsync()
        {
            Trace.Debug(TraceCategory, "callback.begin");
            var context = _contextFactory.CreateProxyContext(HttpContext);
            AsyncManager.OutstandingOperations.Increment();
            _requestHandler.SecurityCallbackAsync(context)
                .ContinueWith(t =>
                {
                    try
                    {
                        // TODO explore status of task
                    }
                    finally
                    {
                        AsyncManager.OutstandingOperations.Decrement();
                    }
                });
        }

        /// <summary>
        /// Action completed
        /// </summary>
        /// <returns> Action result</returns>
        public ActionResult Oauth_CallbackCompleted()
        {
            Trace.Debug(TraceCategory, "callback.end");
            return Content("You can safely close this window");
        }

        #endregion

        /// <summary>
        /// Handles exceptions
        /// </summary>
        /// <param name="filterContext"> Filter context. </param>
        protected override void OnException(ExceptionContext filterContext)
        {
            // Fail if we can't do anything; app will crash.
            if (filterContext == null)
            {
                return;
            }

            var ex = filterContext.Exception ?? new Exception("No further information exists.");
            Trace.Error(TraceCategory, "Unable to process", ex);

            // TODO redirect to error page
            /*filterContext.ExceptionHandled = true;
            var data = new ErrorPresentation
            {
                ErrorMessage = HttpUtility.HtmlEncode(ex.Message),
                TheException = ex,
                ShowMessage = !(filterContext.Exception == null),
                ShowLink = false
            };
            filterContext.Result = View("ErrorPage", data);*/
        }

        /// <summary>
        /// Creates response DTO
        /// </summary>
        /// <param name="code"> Result code. </param>
        /// <param name="message"> Error message. </param>
        /// <returns> Response DTO </returns>
        private ErrorResult GetErrorResult(int code, string message)
        {
           return new ErrorResult
            {
                Id = string.Empty,
                Error =
                    new ErrorDetails
                    {
                        Code = code,
                        Message = message
                    }
            };
        }
    }
}