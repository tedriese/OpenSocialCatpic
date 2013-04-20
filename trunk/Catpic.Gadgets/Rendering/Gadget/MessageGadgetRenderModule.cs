// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageGadgetRenderModule.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Renders message script and localizes content
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Rendering.Gadget
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Catpic.Gadgets.Containers;
    using Catpic.Utils;
    using Catpic.Utils.Diagnostic;

    using HtmlAgilityPack;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Renders message script and localizes content
    /// </summary>
    public class MessageGadgetRenderModule : IGadgetRenderModule
    {
        /// <summary>
        /// Trace category
        /// </summary>
        private const string TraceCategory = "render.module.message";

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
            
            // TODO process language direction
            var language = gadget.Context.Language.ToLower();
            
            // no localization required           
            if (!gadget.Definition.ModulePreferences.Locales.Any(l => l.Language.ToLower() == language))
            {
                Trace.Debug(TraceCategory, string.Format("end {0}", gadget.Context.Uri));
                return AsyncHelper.GetEmptyTask();
            }

            var locale = gadget.Definition.ModulePreferences.Locales.First(l => l.Language.ToLower() == language);

            // TODO analyze this approach
            Action endRenderingAction = () =>
                    {
                        IDictionary<string, string> messages = locale.Messages.InnerDictionary;

                        // TODO cache
                        var nodePaths = gadget.Context.RenderMode == RenderModeType.Iframe
                               ? new[] { "html/head", "html/body" }
                               : new[] { "div" };
                        ProcessNodes(document, messages, nodePaths);

                        var head = gadget.Context.RenderMode == RenderModeType.Iframe
                                       ? document.DocumentNode.SelectSingleNode("html/head")
                                       : document.DocumentNode.SelectSingleNode("div");

                        // insert messages script
                        head.AppendChild(GetMessagesScript(document, messages));
                        Trace.Debug(TraceCategory, string.Format("end {0}", gadget.Context.Uri));
                    };

            // no need to initialize
            if (locale.Messages.IsReady)
            {
                endRenderingAction();
                return AsyncHelper.GetEmptyTask();
            }

            // io operation is possible here
            return locale.Messages.InitializeAsync().ContinueWith(t => endRenderingAction(), TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// Processes nodes
        /// </summary>
        /// <param name="document"> Target html document. </param>
        /// <param name="messages"> Mmessages map. </param>
        /// <param name="nodePaths"> Nnode paths. </param>
        private static void ProcessNodes(
            HtmlDocument document, IDictionary<string, string> messages, IEnumerable<string> nodePaths)
        {
            foreach (var path in nodePaths)
            {
                // replace __MSG_*_ in head and body
                var head = document.DocumentNode.SelectSingleNode(path);
                
                // localize'em all
                LocalizeNode(head, messages);
            }
        }

        /// <summary>
        /// Localizes nodes
        /// </summary>
        /// <param name="node"> The node. </param>
        /// <param name="messages"> The messages. </param>
        private static void LocalizeNode(HtmlNode node, IDictionary<string, string> messages)
        {
            node.InnerHtml = GadgetConsts.MessageRegex.Replace(
                node.InnerHtml,
                delegate(Match match)
                    {
                    string key = match.Groups[1].Value;
                    return messages.ContainsKey(key) ? JsonHelper.Normalize(messages[key]) : key;
                });
        }

        /// <summary>
        /// Gets messages script.
        /// </summary>
        /// <param name="document"> Target Html document. </param>
        /// <param name="messages"> Messages map. </param>
        /// <returns> Html node</returns>
        private static HtmlNode GetMessagesScript(HtmlDocument document, IDictionary<string, string> messages)
        {
            var messagesScript = document.CreateElement("script");
            var json = JsonConvert.SerializeObject(messages, new KeyValuePairConverter());
            messagesScript.InnerHtml = string.Format("gadgets.Prefs.setMessages_({0});", json);
            return messagesScript;
        }
    }
}
