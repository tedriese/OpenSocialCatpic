// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UtilGadgetRenderModule.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Injects util scripts; should be after all other scripts
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Rendering.Gadget
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Catpic.Gadgets.Containers;
    using Catpic.Gadgets.Format;
    using Catpic.Gadgets.Security;
    using Catpic.Utils;
    using Catpic.Utils.Caching;
    using Catpic.Utils.Diagnostic;

    using HtmlAgilityPack;

    using Newtonsoft.Json;

    /// <summary>
    /// Injects util scripts; should be after all other scripts
    /// </summary>
    public class UtilGadgetRenderModule : IGadgetRenderModule
    {
        /// <summary>
        /// Trace category
        /// </summary>
        private const string TraceCategory = "render.module.util";

        /// <summary>
        /// Trace instance
        /// </summary>
        private static readonly ITrace Trace = TraceFactory.GetTrace();

        /// <summary>
        /// Preloads cache
        /// </summary>
        private readonly ICache _preloadCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="UtilGadgetRenderModule"/> class.
        /// </summary>
        /// <param name="preloadCache"> Preload cache instance. </param>
        public UtilGadgetRenderModule(ICache preloadCache)
        {
            this._preloadCache = preloadCache;
        }

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

            var tasks = new List<Task>();
            foreach (var preload in gadget.Definition.ModulePreferences.Preloads)
            {
                var cacheEntry = GetCacheKey(preload, gadget.Context.SecurityToken);

                // no preload in cache
                if (!this._preloadCache.Contains(cacheEntry))
                {
                    PreloadDefinition closure = preload;
                    PreloadCacheEntry cacheClosure = cacheEntry;
                    
                    // fetch data like in makeRequest
                    // TODO refactoring: apply DRY principle?
                     tasks.Add(RemoteFetchHelper.GetFetchDataTask(preload.Href, "GET", null, TaskCreationOptions.None)
                          .ContinueWith(t =>
                              {
                                  try
                                  {
                                      var response = t.Result;
                                      using (Stream responseStream = response.GetResponseStream())
                                      {
                                          var reader = new StreamReader(responseStream);
                                          var strResponse = reader.ReadToEnd();
                                          var jsonResponse = JsonConvert.SerializeObject(
                                                  new
                                                  {
                                                      id = closure.Href.ToString(),
                                                      rc = 200,
                                                      body = strResponse
                                                  });
                                          this._preloadCache.Add(cacheClosure, jsonResponse);
                                      }
                                  }
                                  catch (AggregateException ex)
                                  {
                                      if (ex.InnerException != null)
                                      {
                                          Trace.Error(TraceCategory, string.Format("preload: {0}", closure.Href), ex.InnerException);
                                      }
                                  }
                                  catch (Exception ex)
                                  {
                                      Trace.Error(TraceCategory, string.Format("preload: {0}", closure.Href), ex);
                                  }
                              }));
                 }
            }

            Task taskResult;
            if (tasks.Count > 0)
            {
                var cts = new CancellationTokenSource();
                var tf = new TaskFactory(
                    cts.Token,
                    TaskCreationOptions.AttachedToParent,
                    TaskContinuationOptions.ExecuteSynchronously,
                                         TaskScheduler.Default);

                // wait for loading of all preloads
                taskResult = tf.ContinueWhenAll(tasks.ToArray(), t => { });
            }
            else
            {
                taskResult = AsyncHelper.GetEmptyTask();
            }

            return taskResult.ContinueWith(
                t =>
                    {
                        Render(container, gadget, document, this._preloadCache);
                        Trace.Debug(TraceCategory, string.Format("end {0}", gadget.Context.Uri));
                    });
        }

        /// <summary>
        /// Renders gadget content into html document. TODO: optimize solution
        /// </summary>
        /// <param name="container"> Container instance.  </param>
        /// <param name="gadget"> Gadget instance.  </param>
        /// <param name="document"> Target html document  </param>
        /// <param name="preloadCache"> Preload cache. </param>
        private static void Render(IContainer container, Catpic.Gadgets.Gadget gadget, HtmlDocument document, ICache preloadCache)
        {
            // replace module id with default value
            // NOTE explore the purpose of moduleId and use the corresponding one
            ReplaceModuleId(document.DocumentNode, "0");

            // init script
            var initScript = document.CreateElement("script");

            // get flat list of features
            var features = new List<Feature>();
            features.AddRange(gadget.Definition.ModulePreferences.RequiredFeatures);
            features.AddRange(gadget.Definition.ModulePreferences.OptionalFeatures);
            
            // get container settings and add core.util with features settings
            string containerSettings = container.ContainerSettings;

            // set preloads
            var preloadsBuilder = new StringBuilder();
            var delimiter = string.Empty;
            foreach (var preload in gadget.Definition.ModulePreferences.Preloads)
            {
                var cacheEntry = GetCacheKey(preload, gadget.Context.SecurityToken);
                if (preloadCache.Contains(cacheEntry))
                {
                    var response = preloadCache.Get(cacheEntry) as string;
                    preloadsBuilder.AppendFormat("{0}{1}", delimiter, response);
                    delimiter = ",";
                }
            }

            string preloadScript = string.Empty;
            if (delimiter != string.Empty)
            {
                preloadScript = string.Format("gadgets.io.preloaded_ = [{0}];", preloadsBuilder);
            }

            // TODO cache string per container
            var format = string.Format("{0},{1}}}", containerSettings.Substring(0, containerSettings.Length - 1), "{0}");

            initScript.InnerHtml += string.Format("gadgets.config.init({0}); {1}", GetFeaturesParameters(format, features), preloadScript);
            
            // run onLoad script handlers
            var onLoadScript = document.CreateElement("script");
            onLoadScript.InnerHtml += "gadgets.util.runOnLoadHandlers();";

            // attach these scripts last
            var path = gadget.Context.RenderMode == RenderModeType.Iframe ? "html/body" : "div";
            var body = document.DocumentNode.SelectSingleNode(path);

            // TODO: move this script to head?
            if (gadget.Context.RenderMode == RenderModeType.Iframe)
            {
                // should be first
                body.PrependChild(initScript); 
            }
            else
            {
                // no body tag
                body.AppendChild(initScript); 
            }
            
            body.AppendChild(onLoadScript); // should be last
        }

        /// <summary>
        /// Replaces module id
        /// </summary>
        /// <param name="node"> Html node. </param>
        /// <param name="moduleId"> Module id. </param>
        private static void ReplaceModuleId(HtmlNode node, string moduleId)
        {
            node.InnerHtml = GadgetConsts.ModuleIdRegex.Replace(node.InnerHtml, moduleId);
        }

        /// <summary>
        /// Creates init string.
        /// </summary>
        /// <param name="format"> Format string with other settings. </param>
        /// <param name="features"> List of features. </param>
        /// <returns> Valid init string. </returns>
        private static string GetFeaturesParameters(string format, IEnumerable<Feature> features)
        {
            var builder = new StringBuilder(256);

            builder.Append("\"core.util\":{");
            var delimiterOut = string.Empty;
            foreach (var feature in features)
            {
                builder.AppendFormat("{0}\"{1}\":{{", delimiterOut, feature.Name);

                // iterate parameters of the feature
                var delimiterInt = string.Empty;
                foreach (var key in feature.Parameteres.Keys)
                {
                    builder.AppendFormat("\"{0}\":\"{1}\"{2}", key, feature.Parameteres[key], delimiterInt);
                    delimiterInt = ",";
                }

                builder.Append("}");
                delimiterOut = ",";
            }

            builder.Append("}");

            // TODO: remove commas after the latest elements)
            return format.Replace("{0}", builder.ToString());
        }

        /// <summary>
        /// Returns cache key for preload entry
        /// </summary>
        /// <param name="preload"> Preload entry. </param>
        /// <param name="token"> Security token. </param>
        /// <returns> Cache key</returns>
        private static PreloadCacheEntry GetCacheKey(PreloadDefinition preload, ISecurityToken token)
        {
            return new PreloadCacheEntry
                {
                    Url = preload.Href.ToString(),
                    Owner = token.OwnerId,
                    Viewer = token.ViewerId,
                };
        }

        /// <summary>
        /// Represents preload cache entry
        /// </summary>
        private struct PreloadCacheEntry
        {
            /// <summary>
            /// Gets or sets preload's url
            /// </summary>
            public string Url { get; set; }

            /// <summary>
            /// Gets or sets Owner.
            /// </summary>
            public string Owner { get; set; }

            /// <summary>
            /// Gets or sets Viewer.
            /// </summary>
            public string Viewer { get; set; }

            /// <summary>
            /// Returns string representation of object.
            /// </summary>
            /// <returns> String representation of object. </returns>
            public override string ToString()
            {
                return string.Format("{0}:{1}:{2}", this.Url, this.Owner, this.Viewer);
            }
        }
    }
}
