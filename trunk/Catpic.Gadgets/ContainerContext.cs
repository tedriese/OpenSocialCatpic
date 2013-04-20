// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainerContext.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents container context which is used by container rendering pipeline
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;

    using Catpic.Gadgets.Security;
    using Catpic.Utils;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents container context which is used by container rendering pipeline
    /// </summary>
    public class ContainerContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerContext"/> class.
        /// </summary>
        /// <param name="context"> Actual HttpContext. </param>
        /// <param name="securityToken"> Security token. </param>
        public ContainerContext(HttpContextBase context, ISecurityToken securityToken)
        {
            this.Http = context;
            this.SecurityToken = securityToken;
            var request = context.Request;

            // NOTE detect action by request
            string path = request.Path.ToLower();
            if (path.Contains("environment"))
            {
                this.InitializeFromInitializeCall(request);
            }
            else if (path.Contains("metadata"))
            {
                this.InitializeFromMetadataCall(request);
            }
        }

        /// <summary>
        /// Gets name of container.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets view name.
        /// </summary>
        public string View { get; private set; }

        /// <summary>
        /// Gets preffered language.
        /// </summary>
        public string Language { get; private set; }

        /// <summary>
        /// Gets country.
        /// </summary>
        public string Country { get; private set; }

        /// <summary>
        /// Gets Http context.
        /// </summary>
        public HttpContextBase Http { get; private set; }

        /// <summary>
        /// Gets a value indicating whether cache is enabled.
        /// </summary>
        public bool IsCacheEnabled { get; private set; }

        /// <summary>
        /// Gets container specific action.
        /// </summary>
        public string Action { get; private set; }

        /// <summary>
        /// Gets the list of modules.
        /// </summary>
        public IEnumerable<Module> Modules { get; private set; }

        /// <summary>
        /// Gets SecurityToken.
        /// </summary>
        public ISecurityToken SecurityToken { get; private set; }

        /// <summary>
        /// Initializes container context for environment call
        /// </summary>
        /// <param name="request"> Http request. </param>
        private void InitializeFromInitializeCall(HttpRequestBase request)
        {
            var qs = request.QueryString;
            this.Action = "initialize";
            this.Name = qs["container"];

            string nocache = qs["nocache"] ?? "0";
            this.IsCacheEnabled = nocache == "0";
        }

        /// <summary>
        /// Initializes contaienr contex for metadata call
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        private void InitializeFromMetadataCall(HttpRequestBase request)
        {
            var qs = request.QueryString;
            string nocache = qs["nocache"] ?? "0";
            this.IsCacheEnabled = nocache == "0";

            this.Action = "metadata";
            using (var reader = new StreamReader(request.InputStream))
            {
                var body = reader.ReadToEnd();
                var jsonBody = JsonConvert.DeserializeObject<JObject>(body);
                
                // TODO assert values
                var jsonContext = jsonBody["context"];
                this.Name = jsonContext["container"].Value<string>();
                this.View = jsonContext["view"].Value<string>();
                this.Language = jsonContext["language"].Value<string>();
                this.Country = jsonContext["country"].Value<string>();
                
                // process metadata of several gadgets (context-definition pairs)
                var modules = new List<Module>();
                foreach (var jsonGadget in jsonBody["gadgets"])
                {
                    var uri = jsonGadget["url"].Value<string>();
                    var resolvedUri = FileHelper.ResolvePath(uri);
                    var moduleId = jsonGadget["moduleId"].Value<int>();
                    modules.Add(new Module(moduleId, new Uri(resolvedUri)));
                }

                this.Modules = modules;
            }
        }
    }
}
