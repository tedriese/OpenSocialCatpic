// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetadataContainerRenderModule.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Renders metadata content
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Rendering.Container
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Catpic.Gadgets.Containers;
    using Catpic.Gadgets.Format;
    using Catpic.Utils;
    using Catpic.Utils.Caching;
    using Catpic.Utils.Diagnostic;

    using Newtonsoft.Json;

    /// <summary>
    /// Renders metadata content
    /// </summary>
    public class MetadataContainerRenderModule : IContainerRenderModule
    {
        /// <summary>
        /// Trace category
        /// </summary>
        private const string TraceCategory = "container.render.metadata";

        /// <summary>
        /// Trace instance
        /// </summary>
        private static readonly ITrace Trace = TraceFactory.GetTrace();

        /// <summary>
        /// Default json serializator settings
        /// </summary>
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings();

        /// <summary>
        /// Stores processed metadata entries
        /// </summary>
        private readonly ICache _metadataCache;

        /// <summary>
        /// Initializes static members of the <see cref="MetadataContainerRenderModule"/> class.
        /// </summary>
        static MetadataContainerRenderModule()
        {
            JsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataContainerRenderModule"/> class.
        /// </summary>
        /// <param name="metadataCache">
        /// The metadata cache.
        /// </param>
        public MetadataContainerRenderModule(ICache metadataCache)
        {
            this._metadataCache = metadataCache;
        }

        /// <summary>
        /// Renders container specific content into StringBuilder object
        /// </summary>
        /// <param name="container"> Container instance. </param>
        /// <param name="context"> Context context. </param>
        /// <param name="content"> Output writer. </param>
        /// <returns> Async task</returns>
        public Task RenderAsync(IContainer container, ContainerContext context, StringBuilder content)
        {
            Trace.Debug(TraceCategory, "metadata.begin");
            if (context.Action == "metadata")
            {
                var tasks = this.CreateRequestTasks(container, context).ToArray();
                if (tasks.Length > 0)
                {
                    // TODO implement cancelation and error handling logic
                    var cts = new CancellationTokenSource();
                    var tf = new TaskFactory(
                        cts.Token,
                        TaskCreationOptions.AttachedToParent,
                        TaskContinuationOptions.ExecuteSynchronously,
                        TaskScheduler.Default);
                    return tf.ContinueWhenAll(
                        tasks,
                        completedTasks => this.FinishRender(container, context, content),
                        TaskContinuationOptions.ExecuteSynchronously);
                }

                // all gadgets are cached
                this.FinishRender(container, context, content);
                return AsyncHelper.GetEmptyTask();
            }

            return AsyncHelper.GetEmptyTask();
        }

        /// <summary>
        /// Localize text message
        /// </summary>
        /// <param name="text"> Text message. </param>
        /// <param name="locale"> Current locale. </param>
        /// <returns> New text</returns>
        private static string LocalizeText(string text, LocaleDefinition locale)
        {
            // cannot localize
            if (locale == null)
            {
                return text;
            }

            var messages = locale.Messages.InnerDictionary;
            return GadgetConsts.MessageRegex.Replace(
                text,
                delegate(Match match)
                    {
                    string key = match.Groups[1].Value;
                    return messages.ContainsKey(key) ? JsonHelper.Normalize(messages[key]) : key;
                });
        }

        /// <summary>
        /// Finishes rendering
        /// </summary>
        /// <param name="container"> Container instance. </param>
        /// <param name="context"> Container context. </param>
        /// <param name="content"> Output content builder. </param>
        private void FinishRender(IContainer container, ContainerContext context, StringBuilder content)
        {
            Trace.Debug(TraceCategory, "render.start");

            IList<ExpandoObject> gadgets = new List<ExpandoObject>();
            foreach (var module in context.Modules)
            {
                // metadata entry present in cache
                if (this._metadataCache.Contains(this.GetMetadataCacheKey(module)))
                {
                    gadgets.Add(this._metadataCache.Get(this.GetMetadataCacheKey(module)) as ExpandoObject);
                }
                else
                {
                    // try to find gadget definition
                    var gadgetDefinition = container.GadgetFactory.Get(module.Uri);
                    if (gadgetDefinition != null)
                    {
                        var gadgetMetadata = this.GetGadgetMetadata(gadgetDefinition);
                        this._metadataCache.Add(this.GetMetadataCacheKey(module), gadgetMetadata);
                        gadgets.Add(gadgetMetadata);
                    }
                    else
                    {
                        // no metadata for this gadget
                        // TODO investigate what should be returned in this case
                        Trace.Warn(string.Format("metadata: gadget isn't found in cache: {0}", module.Uri));
                    }
                }
            }

            var metaStr = JsonConvert.SerializeObject(new MetadataResponse { Gadgets = gadgets }, JsonSettings);
            content.Append(metaStr);

            Trace.Debug(TraceCategory, metaStr);
            Trace.Debug(TraceCategory, "render.end");
            Trace.Debug(TraceCategory, "metadata.end");
        }

        /// <summary>
        /// Returns DTO object which represents gadget metadata
        /// </summary>
        /// <param name="gadgetDefinition"> Gadget definition.  </param>
        /// <returns> DTO instance.  </returns>
        private ExpandoObject GetGadgetMetadata(GadgetDefinition gadgetDefinition)
        {
            var expando = new ExpandoObject();
            var metadata = expando as IDictionary<string, object>;

            // TODO pay respect to container locale settings
            LocaleDefinition locale = null;
            if (gadgetDefinition.ModulePreferences.Locales.Any(l => l.Language.ToLower() == "en"))
            {
                locale = gadgetDefinition.ModulePreferences.Locales.First(l => l.Language.ToLower() == "en");
            }

            // TODO render iframeUrl here:
            // "iframeUrl":"\/gadgets\/ifr?container=default&v=642b5cce834ee370b396f18811c8aac2&lang=default&country=default&view=&up_default_mode=0&up_persist_memory=0&up_memory=0&url=http%3A%2F%2Fwww.google.com%2Fig%2Fmodules%2Fcalculator.xml"

            metadata["features"] = gadgetDefinition.ModulePreferences.RequiredFeatures.Select(f => f.Name);

            // user preferences
            var eoUserPref = new ExpandoObject();
            var mapUserPref = eoUserPref as IDictionary<string, object>;
            foreach (var userPref in gadgetDefinition.UserPreferences)
            {
                // TODO localize metadata using LocalizeText() method
                // it seems like we should create copy of userPreference here to substitute localization variables
                mapUserPref[userPref.Name] = userPref;
            }

            metadata["userPrefs"] = eoUserPref;

            // header
            var header = gadgetDefinition.ModulePreferences.Header;
            foreach (var key in header.Keys)
            {
                metadata[key] = LocalizeText(header[key], locale);
            }

            return expando;
        }

        /// <summary>
        /// Returns key for metadata cache
        /// </summary>
        /// <param name="module"> Gadget module. </param>
        /// <returns> Cache key</returns>
        private string GetMetadataCacheKey(Module module)
        {
            return string.Format("metadata.{0}", module.Uri);
        }

        /// <summary>
        /// Creates request tasks
        /// </summary>
        /// <param name="container"> The container instance. </param>
        /// <param name="context"> The container context. </param>
        /// <returns> Async task </returns>
        private IEnumerable<Task> CreateRequestTasks(IContainer container, ContainerContext context)
        {
            foreach (var module in context.Modules)
            {
                // TODO check isCacheEnabled key
                if ((!this._metadataCache.Contains(module.Uri)) && container.GadgetFactory.Get(module.Uri) == null)
                {
                    Trace.Debug(TraceCategory, string.Format("queue fetching {0}", module.Uri));
                    yield return RemoteFetchHelper.GetFetchDataTask(module.Uri, "GET", null, TaskCreationOptions.None)
                        .ContinueWith(t =>
                        {
                            try
                            {
                                Trace.Debug(TraceCategory, "metadata: parse response");
                                var response = t.Result;
                                
                                // TODO refactoring this!!!
                                GadgetDefinition gadgetDefinition = container.GadgetFactory.Create(module.Uri, response);
                                Trace.Debug(TraceCategory, "metadata: write json");
                                LocaleDefinition locale = null;
                                if (gadgetDefinition.ModulePreferences.Locales.Any(l => l.Language.ToLower() == "en"))
                                {
                                    locale =
                                        gadgetDefinition.ModulePreferences.Locales.First(
                                            l => l.Language.ToLower() == "en");
                                }

                                // io operation is possible here, make it async to prevent thread blocking
                                if (locale != null)
                                {
                                    return locale.Messages.InitializeAsync();
                                }

                                return AsyncHelper.GetEmptyTask();
                            }
                            catch (Exception ex)
                            {
                                Trace.Error(TraceCategory, string.Format("fetch: unable to process {0}", module.Uri), ex);
                                return Task.Factory.StartNew(() => { });
                            }
                        });
                }
            }
        }

        /// <summary>
        /// Represents metadata response DTO
        /// </summary>
        [DataContract]
        private class MetadataResponse
        {
            /// <summary>
            /// Gets or sets Gadgets.
            /// </summary>
            [DataMember(Name = "gadgets")]
            public IEnumerable<ExpandoObject> Gadgets { get; set; }
        }
    }
}
