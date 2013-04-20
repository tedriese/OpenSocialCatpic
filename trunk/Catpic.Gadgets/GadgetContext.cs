// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GadgetContext.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Default implementation of GadgetContext
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Web;

    using Catpic.Gadgets.Rendering;
    using Catpic.Gadgets.Security;
    using Catpic.Utils;

    /// <summary>
    /// Default implementation of GadgetContext
    /// </summary>
    public class GadgetContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GadgetContext"/> class.
        /// </summary>
        /// <param name="context"> Http context. </param>
        /// <param name="securityToken"> Security token. </param>
        public GadgetContext(HttpContextBase context, ISecurityToken securityToken)
        {
            this.Http = context;
            this.SecurityToken = securityToken;
            var request = context.Request;

            switch (request.HttpMethod)
            {
                case "GET":
                    this.InitializeFromGet(request);
                    break;
                case "POST":
                    this.InitializeFromPost(request);
                    break;
            }

            this.UserPrefs = this.GetUserPrefsFromRequest(request);
        }

        #region Properties

        /// <summary>
        /// Gets or sets container name.
        /// </summary>
        public string ContainerName { get; set; }

        /// <summary>
        /// Gets or sets view name.
        /// </summary>
        public string ViewName { get; set; }

        /// <summary>
        /// Gets or sets country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets Language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets ModuleId.
        /// </summary>
        public int ModuleId { get; set; }

        /// <summary>
        /// Gets or sets Uri.
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// Gets or sets Http.
        /// </summary>
        public HttpContextBase Http { get; set; }

        /// <summary>
        /// Gets UserPrefs.
        /// </summary>
        public IDictionary<string, string> UserPrefs { get; private set; }

        /// <summary>
        /// Gets RenderMode.
        /// </summary>
        public RenderModeType RenderMode { get; private set; }

        /// <summary>
        /// Gets a value indicating whether cache is enabled.
        /// </summary>
        public bool IsCacheEnabled { get; private set; }

        /// <summary>
        /// Gets SecurityToken.
        /// </summary>
        public ISecurityToken SecurityToken { get; private set; }

        #endregion

        /// <summary>
        /// Initialized gadget context from GET request
        /// </summary>
        /// <param name="request"> Http request. </param>
        private void InitializeFromGet(HttpRequestBase request)
        {
            // parse query string
            var qs = request.QueryString;
            
            // TODO assert values
            var uri = qs["url"];
            this.ContainerName = qs["container"] ?? "default";
            this.ViewName = qs["view"] ?? "default";
            this.Language = qs["lang"] ?? "ALL";
            this.Country = qs["country"] ?? "ALL";
            
            string nocache = qs["nocache"] ?? "0";
            this.IsCacheEnabled = (nocache == "0");

            // initialize renderMode
            RenderModeType renderModeType;
            RenderModeType.TryParse(qs["renderType"], true, out renderModeType);
            RenderMode = renderModeType;

            // TODO concat requests doesn't have url)
            if (uri != null)
            {
                var resolvedUri = FileHelper.ResolvePath(uri);
                Uri = new Uri(resolvedUri);
            }
        }

        /// <summary>
        /// Initializes gadget context from POST request
        /// </summary>
        /// <param name="request"> Http request. </param>
        private void InitializeFromPost(HttpRequestBase request)
        {
          // TODO explore such cases
        }

        /// <summary>
        /// Gets user preferences from request
        /// </summary>
        /// <param name="request"> Http request. </param>
        /// <returns> User preferences mapping </returns>
        private IDictionary<string, string> GetUserPrefsFromRequest(HttpRequestBase request)
        {
           var userPrefs = new Dictionary<string, string>();
            NameValueCollection parameters = request.HttpMethod == "GET" ? request.QueryString : request.Form;
            if (parameters != null)
            {
                foreach (string parameter in parameters)
                {
                    if (parameter.StartsWith("up_"))
                    {
                        var upName = parameter.Substring(3);
                        if (!userPrefs.ContainsKey(upName))
                        {
                            userPrefs.Add(upName, parameters[parameter]);
                        }
                    }
                }
            }

            return userPrefs;
        }
    }
}
