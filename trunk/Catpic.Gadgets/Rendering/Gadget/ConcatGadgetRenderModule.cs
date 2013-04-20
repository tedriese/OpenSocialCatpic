// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConcatGadgetRenderModule.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Renders concat scripts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Rendering.Gadget
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;

    using Catpic.Gadgets.Containers;
    using Catpic.Utils;
    using Catpic.Utils.Diagnostic;

    using HtmlAgilityPack;

    /// <summary>
    /// Renders concat scripts.
    /// </summary>
    public class ConcatGadgetRenderModule : IGadgetRenderModule
    {
        /// <summary>
        /// Limit of query string
        /// </summary>
        private const int MaxQueryStringScriptLength = 1600;

        /// <summary>
        /// Content type
        /// </summary>
        private const string ContentType = "text/javascript";
        
        /// <summary>
        /// Concat script base path TODO read it from container
        /// </summary>
        private const string ConcatPath = @"/gadgets/concat";

        /// <summary>
        /// Trace category
        /// </summary>
        private const string TraceCategory = "render.module.concat";

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
            Render(container, gadget, document);
            Trace.Debug(TraceCategory, string.Format("end {0}", gadget.Context.Uri));

            return AsyncHelper.GetEmptyTask();
        }

        /// <summary>
        /// Renders concat scripts
        /// </summary>
        /// <param name="container"> Container instance. </param>
        /// <param name="gadget"> Gadget instance. </param>
        /// <param name="document"> Target html document. </param>
        private static void Render(IContainer container, Catpic.Gadgets.Gadget gadget, HtmlDocument document)
        {
            var nodePaths = gadget.Context.RenderMode == RenderModeType.Iframe
                                ? new[] { "html/head", "html/body" }
                                : new[] { "div" };
            ProcessNodes(gadget, document, nodePaths);
        }

        /// <summary>
        /// Processes html nodes
        /// </summary>
        /// <param name="gadget"> Gadget instance. </param>
        /// <param name="document"> Target html document. </param>
        /// <param name="nodePaths"> The node paths. </param>
        private static void ProcessNodes(Catpic.Gadgets.Gadget gadget, HtmlDocument document, IEnumerable<string> nodePaths)
        {
            foreach (var path in nodePaths)
            {
                var node = document.DocumentNode.SelectSingleNode(path);
                ReplaceScripts(document, node, gadget.Context.Uri.ToString());
            }
        }

        /// <summary>
        /// Replaces external script definition with concat script
        /// </summary>
        /// <param name="document"> Target html document. </param>
        /// <param name="node"> Html node</param>
        /// <param name="url"> The url. </param>
        private static void ReplaceScripts(HtmlDocument document, HtmlNode node, string url)
        {
            var scriptSrcRefs = new List<string>();
            var scriptNodes = node.SelectNodes("script");

            // NOTE: actually, core scripts should be present here, but if core module fails, NullReference exception is raised here
            // TODO improve approach?
            if (scriptNodes == null)
            {
                return;
            }

            foreach (var script in scriptNodes)
            {
                var closure = script;
                if (closure.Attributes.Any(a => a.Name == "src"))
                {
                    var src = closure.Attributes.Single(a => a.Name == "src").Value;
                    
                    // TODO check value
                    scriptSrcRefs.Add(src); 
                    
                    // note will be replaced with concat script
                    node.ChildNodes.Remove(closure);
                }
            }

            var concatScripts = CreateConcatScript(document, scriptSrcRefs, url);
            
            // add scripts
            concatScripts.Reverse();
            concatScripts.ForEach(s => node.PrependChild(s));
        }

        /// <summary>
        /// Creates concat scripts from the list of script refernces lists
        /// </summary>
        /// <param name="document"> Target html document.  </param>
        /// <param name="scriptSrcRefs"> The list of scr references </param>
        /// <param name="url"> The url. </param>
        /// <returns> The list of html nodes </returns>
        private static List<HtmlNode> CreateConcatScript(HtmlDocument document, List<string> scriptSrcRefs, string url)
        {
            var concatScripts = new List<HtmlNode>();

            var tmpList = new List<string>();
            var str = string.Empty;

            // NOTE pay respect to query string limit
            for (int i = 0; i < scriptSrcRefs.Count; i++)
            {
                var scriptSrc = scriptSrcRefs[i];
                var encodedSrc = HttpUtility.UrlEncode(scriptSrc);
                tmpList.Add(encodedSrc);
                str += encodedSrc;

                if (str.Length > MaxQueryStringScriptLength || i == (scriptSrcRefs.Count - 1))
                {
                    // flush scripts
                    var src = BuildConcatUrl(tmpList, url);
                    var script = document.CreateElement("script");
                    script.Attributes.Add("src", src);
                    concatScripts.Add(script);
                    
                    // clean temp variables
                    tmpList.RemoveAll(s => true);
                    str = string.Empty;
                }
            }
            
            return concatScripts;
        }

        /// <summary>
        /// Builds concat url which is sent to client
        /// </summary>
        /// <param name="scriptSrcRefs"> The script src refs. </param>
        /// <param name="url"> The url. </param>
        /// <returns> Concat url</returns>
        private static string BuildConcatUrl(List<string> scriptSrcRefs, string url)
        {
            var sb = new StringBuilder(1024);
            sb.AppendFormat("{0}?rewriteMime={1}&gadget={2}", ConcatPath, ContentType, HttpUtility.UrlEncode(url));
            for (int i = 1; i <= scriptSrcRefs.Count; i++)
            {
                var encodedSrc = scriptSrcRefs[i - 1];
                sb.AppendFormat("&{0}={1}", i, encodedSrc);
            }

            return sb.ToString();
        }  
    }
}
