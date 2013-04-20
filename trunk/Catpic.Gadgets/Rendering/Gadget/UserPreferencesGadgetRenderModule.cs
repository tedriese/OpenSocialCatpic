// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserPreferencesGadgetRenderModule.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Renders user preferences script
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Rendering.Gadget
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Catpic.Gadgets.Containers;
    using Catpic.Utils;
    using Catpic.Utils.Diagnostic;

    using HtmlAgilityPack;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Renders user preferences script
    /// </summary>
    public class UserPreferencesGadgetRenderModule : IGadgetRenderModule
    {
        /// <summary>
        /// Trace category
        /// </summary>
        private const string TraceCategory = "render.module.userpref";

        /// <summary>
        /// Trace instance
        /// </summary>
        private static readonly ITrace Trace = TraceFactory.GetTrace();

        /// <summary>
        /// Regex for user preference substitution
        /// </summary>
        private static readonly Regex UserPrefRegex = new Regex(@"__UP_(\w*?)__", RegexOptions.Compiled);

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
            Render(container, gadget, document);
            Trace.Debug(TraceCategory, string.Format("end {0}", gadget.Context.Uri));

            return AsyncHelper.GetEmptyTask();
        }

        /// <summary>
        /// Renders gadget content into html document
        /// </summary>
        /// <param name="container"> Container instance. </param>
        /// <param name="gadget"> Gadget instance. </param>
        /// <param name="document"> Target html document </param>
        private static void Render(IContainer container, Catpic.Gadgets.Gadget gadget, HtmlDocument document)
        {
            var onLoadScript = document.CreateElement("script");
            var userPrefs = GetUserPref(gadget);
            onLoadScript.InnerHtml = GetDefaultPreferencesScript(userPrefs);

            var path = gadget.Context.RenderMode == RenderModeType.Iframe ? "html/body" : "div";
            var body = document.DocumentNode.SelectSingleNode(path);

            SubstituteUserPrefs(body, userPrefs);
            body.AppendChild(onLoadScript);
        }

        /// <summary>
        /// Gets user preferences map.
        /// </summary>
        /// <param name="gadget"> Ggadget instance. </param>
        /// <returns> User preferences map</returns>
        private static IDictionary<string, string> GetUserPref(Catpic.Gadgets.Gadget gadget)
        {
            var prefsDic = new Dictionary<string, string>();
            foreach (var userPreference in gadget.Definition.UserPreferences)
            {
                var closure = userPreference;
                string value = gadget.Context.UserPrefs.ContainsKey(closure.Name) ?
                    gadget.Context.UserPrefs[closure.Name] : closure.DefaultValue.ToString();
                prefsDic.Add(closure.Name, value);
            }

            return prefsDic;
        }

        /// <summary>
        /// Gets default preferences script
        /// </summary>
        /// <param name="prefsDic"> User preferences map</param>
        /// <returns> String representation of default preferences</returns>
        private static string GetDefaultPreferencesScript(IDictionary<string, string> prefsDic)
        {
            var json = JsonConvert.SerializeObject(prefsDic, new KeyValuePairConverter());
            return string.Format("gadgets.Prefs.setDefaultPrefs_({0});", json);
        }

        /// <summary>
        /// Substitutes use preferences
        /// </summary>
        /// <param name="node"> Html node </param>
        /// <param name="prefsDic"> User preferences map </param>
        private static void SubstituteUserPrefs(HtmlNode node, IDictionary<string, string> prefsDic)
        {
            // NOTE enum may require different processing
            node.InnerHtml = UserPrefRegex.Replace(
                node.InnerHtml,
                delegate(Match match)
                    {
                    string key = match.Groups[1].Value;
                    return prefsDic.ContainsKey(key) ? prefsDic[key] : key;
                });
        }
    }
}
