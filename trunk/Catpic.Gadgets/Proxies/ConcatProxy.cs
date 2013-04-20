// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConcatProxy.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Concatenats scripts which are defined in request to single response
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Proxies
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Catpic.Utils;
    using Catpic.Utils.Diagnostic;

    /// <summary>
    /// Concatenats scripts which are defined in request to single response.
    /// </summary>
    public class ConcatProxy : IConcatProxy
    {
        /// <summary>
        /// Trace category.
        /// </summary>
        private const string TraceCategory = "concat";
        
        /// <summary>
        /// Trace instance.
        /// </summary>
        private static readonly ITrace Trace = TraceFactory.GetTrace();

        /// <summary>
        /// Renders scripts
        /// </summary>
        /// <param name="context"> Gadget context. </param>
        /// <returns> Async task </returns>
        public Task RenderScriptAsync(GadgetContext context)
        {
            Trace.Debug(TraceCategory, "BeginRenderScript");

            var scripts = new List<string>();
            for (int i = 1; i < ushort.MaxValue; i++)
            {
                var url = context.Http.Request.QueryString[i.ToString()];
                if (string.IsNullOrEmpty(url))
                {
                    break;
                }
                scripts.Add(url);
            }

            var cts = new CancellationTokenSource();
            var tf = new TaskFactory(
                cts.Token,
                TaskCreationOptions.AttachedToParent,
                TaskContinuationOptions.ExecuteSynchronously,
                                     TaskScheduler.Default);

            var tasks = new List<Task>();
            foreach (var script in scripts)
            {
                Trace.Debug(TraceCategory, string.Format("fetch data from {0}", script));
                
                // TODO imporove al.
                Task task;
                if (script.StartsWith("http"))
                {
                    task = RemoteFetchHelper.GetFetchDataTask(new Uri(script), "GET", null, TaskCreationOptions.AttachedToParent);
                }
                else
                {
                    task = FileHelper.GetFetchDataTask(script, null, TaskCreationOptions.AttachedToParent);
                }
                tasks.Add(task);
            }

            for (int i = 0; i < tasks.Count; i++)
            {
                tasks[i].ContinueWith(
                    t =>
                        {
                            cts.Cancel();

                            // TODO write the reason
                            Trace.Error(TraceCategory, "Unable to fetch resource", null);
                        },
                    TaskContinuationOptions.OnlyOnFaulted);
            }

            return tf.ContinueWhenAll(
                tasks.ToArray(),
                completedTasks =>
                    {
                        foreach (var completedTask in completedTasks)
                {
                    try
                    {
                        if (completedTask is Task<WebResponse>)
                        {
                            // read response and write it to writer
                            var response = (completedTask as Task<WebResponse>).Result;
                            using (Stream responseStream = response.GetResponseStream())
                            {
                                var reader = new StreamReader(responseStream);
                                var strResponse = reader.ReadToEnd();
                                context.Http.Response.Output.Write(strResponse);
                            }
                        }
                        else
                        {
                            // local file script
                            var task = completedTask as Task<string>;
                            context.Http.Response.Write(task.Result);
                        }
                    }
                    catch (AggregateException aggException)
                    {
                        // TODO log exceptions
                        Trace.Error(TraceCategory, "some of tasks are failed", aggException);
                    }
                    catch (Exception ex)
                    {
                        // TODO log exceptions
                        Trace.Error(TraceCategory, "Unknown exception", ex);
                    }
                }
                    },
                TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
