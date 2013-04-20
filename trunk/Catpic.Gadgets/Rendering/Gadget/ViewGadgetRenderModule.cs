// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewGadgetRenderModule.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Renders content of the view
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Rendering.Gadget
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Catpic.Gadgets.Containers;
    using Catpic.Utils;
    using Catpic.Utils.Diagnostic;

    using HtmlAgilityPack;

    /// <summary>
    /// Renders content of the view
    /// </summary>
    public class ViewGadgetRenderModule : IGadgetRenderModule
    {
        /// <summary>
        /// Trace category
        /// </summary>
        private const string TraceCategory = "render.module.view";

        /// <summary>
        /// Trace instance
        /// </summary>
        private static readonly ITrace Trace = TraceFactory.GetTrace();

        /// <summary>
        /// Renders gadget content into html document
        /// </summary>
        /// <param name="container">Container instance.</param>
        /// <param name="gadget">Gadget instance.</param>
        /// <param name="document">Target html document</param>
        /// <returns>Async task</returns>
        public Task RenderAsync(IContainer container, Catpic.Gadgets.Gadget gadget, HtmlDocument document)
        {
            Trace.Debug(TraceCategory, string.Format("begin {0}", gadget.Context.Uri));
            var viewName = gadget.Context.ViewName;

            var name = gadget.Definition.Views.Any(v => v.Name == viewName)
                           ? viewName
                           : gadget.Definition.Views.First().Name;
            var views = gadget.Definition.Views.Where(v => v.Name == name);

            return this.GetViewsContent(gadget, views).ContinueWith(t => this.SetBody(gadget, document, views));
        }

        /// <summary>
        /// Gets async list of content fetching tasks.
        /// </summary>
        /// <param name="gadget"> target gadget.  </param>
        /// <param name="views"> Views list.  </param>
        /// <returns> Async task. </returns>
        private Task GetViewsContent(Catpic.Gadgets.Gadget gadget, IEnumerable<View> views)
        {
            var cts = new CancellationTokenSource();
            var tf = new TaskFactory(
                cts.Token,
                TaskCreationOptions.AttachedToParent,
                TaskContinuationOptions.ExecuteSynchronously,
                                     TaskScheduler.Default);

            var tasks = new List<Task>();
            foreach (var view in views)
            {
                var closure = view;
                if (closure.Type == View.ViewType.Url && string.IsNullOrEmpty(closure.Content))
                {
                    tasks.Add(RemoteFetchHelper.GetFetchDataTask(closure.Href, "GET", null, TaskCreationOptions.None)
                        .ContinueWith(t =>
                        {
                            try
                            {
                                var response = t.Result;

                                // TODO explore results
                                using (var reader = new StreamReader(response.GetResponseStream()))
                                {
                                    var content = reader.ReadToEnd();
                                    closure.Content = content;
                                }
                            }
                            catch (Exception ex)
                            {
                                Trace.Error(
                                    TraceCategory,
                                    string.Format("gagdet:{0} name={1} view:{2}", gadget.Context.Uri, closure.Name, closure.Href),
                                    ex);
                            }
                        }));
                }
            }

            if (tasks.Count > 0)
            {
                return tf.ContinueWhenAll(tasks.ToArray(), t => { });
            }

            return AsyncHelper.GetEmptyTask();
        }

        /// <summary>
        /// Sets body to html document
        /// </summary>
        /// <param name="gadget"> Gadget instance. </param>
        /// <param name="document"> Target html document. </param>
        /// <param name="views"> Content views. </param>
        private void SetBody(Catpic.Gadgets.Gadget gadget, HtmlDocument document, IEnumerable<View> views)
        {
            var path = gadget.Context.RenderMode == RenderModeType.Iframe ? "html/body" : "div";
            var body = document.DocumentNode.SelectSingleNode(path);

            var content = new StringBuilder(1024 * 10);
            foreach (var view in views)
            {
                content.Append(view.Content);
            }
            body.InnerHtml = content.ToString();
            Trace.Debug(TraceCategory, "end");
        }
    }
}
