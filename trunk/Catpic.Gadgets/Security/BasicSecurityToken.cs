// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasicSecurityToken.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Defines the BasicSecurityToken type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Security
{
    using System;
    using System.Collections.Generic;

    using Catpic.Utils.OAuth;

    /// <summary>
    /// Basic security token implementation. sindig token example: john.doe:john.doe:appid:cont:url:0:default
    /// </summary>
    public class BasicSecurityToken : ISecurityToken
    {
        /// <summary>
        /// Owner key
        /// </summary>
        private const string OwnerKey = "o";

        /// <summary>
        /// Application key
        /// </summary>
        private const string AppKey = "a";

        /// <summary>
        /// Viewer key
        /// </summary>
        private const string ViewerKey = "v";

        /// <summary>
        /// Domain key
        /// </summary>
        private const string DomainKey = "d";

        /// <summary>
        /// Application url key
        /// </summary>
        private const string AppUrlKey = "u";

        /// <summary>
        /// Module key
        /// </summary>
        private const string ModuleKey = "m";

        /// <summary>
        /// Container key
        /// </summary>
        private const string ContainerKey = "c";

        /// <summary>
        /// Crypro service
        /// </summary>
        private readonly ICryptoService _cryptoService;

        /// <summary>
        /// Encryptated token state
        /// </summary>
        private string _сlientState;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicSecurityToken"/> class.
        /// </summary>
        /// <param name="owner"> The owner. </param>
        /// <param name="viewer"> The viewer. </param>
        /// <param name="app"> The app. </param>
        /// <param name="domain"> The domain. </param>
        /// <param name="appUrl"> The app url. </param>
        /// <param name="moduleId"> The module id. </param>
        /// <param name="container"> The container. </param>
        /// <param name="cryptoService"> The crypto service. </param>
        public BasicSecurityToken(
            string owner,
            string viewer,
            string app,
            string domain,
            string appUrl,
            string moduleId,
            string container,
            ICryptoService cryptoService)
        {
            this.OwnerId = owner;
            this.ViewerId = viewer;
            this.AppId = app;
            this.Domain = domain;
            this.AppUrl = appUrl;
            this.ModuleId = moduleId;
            this.Container = container;

            this._cryptoService = cryptoService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicSecurityToken"/> class.
        /// </summary>
        protected BasicSecurityToken()
        {
        }

        /// <summary>
        /// Gets or sets owner of this gadget
        /// </summary>
        public string OwnerId { get; protected set; }

        /// <summary>
        /// Gets or sets Viewer of this gadget
        /// </summary>
        public string ViewerId { get; protected set; }

        /// <summary>
        /// Gets or sets Application id
        /// </summary>
        public string AppId { get; protected set; }

        /// <summary>
        /// Gets or sets Url where the application lives
        /// </summary>
        public string AppUrl { get; protected set; }

        /// <summary>
        /// Gets or sets Domain of the container 
        /// </summary>
        public string Domain { get; protected set; }

        /// <summary>
        /// Gets or sets Container name
        /// </summary>
        public string Container { get; protected set; }

        /// <summary>
        /// Gets or sets module id of this gadget 
        /// </summary>
        public string ModuleId { get; protected set; }

        /// <summary>
        /// Encrypts token in order to pass to client
        /// </summary>
        /// <returns> Client state. </returns>
        public virtual string ToClientState()
        {
            // do not calc state more than once
            if (string.IsNullOrEmpty(this._сlientState))
            {
                // TODO calc hash
                var appUrl = string.Empty;

                // TODO do not put gadget url to token
                string state = string.Format(
                    "{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}:{11}:{12}:{13}",
                    OwnerKey,
                    this.OwnerId,
                    AppKey,
                    this.AppId,
                    ViewerKey,
                    this.ViewerId,
                    DomainKey,
                    this.Domain,
                    AppUrlKey,
                    appUrl,
                    ModuleKey,
                    this.ModuleId,
                    ContainerKey,
                    this.Container);

                var encryptedState = this._cryptoService.Encrypt(state);
                if (encryptedState != null)
                {
                    this._сlientState = Convert.ToBase64String(encryptedState);
                }
            }

            return this._сlientState;
        }

        /// <summary>
        /// Restores token from client state
        /// </summary>
        /// <param name="state"> Client state. </param>
        public virtual void FromClientState(string state)
        {
            var bytes = Convert.FromBase64String(state);
            var decryptedStr = this._cryptoService.Decrypt(bytes);

            // create map
            var keyValueArray = decryptedStr.Split(':');
            var map = new Dictionary<string, string>();
            var length = keyValueArray.Length;
            string s = string.Empty;
            for (int i = 0; i < length; i++)
            {
                if (i % 2 == 0) 
                {
                    s = keyValueArray[i];
                }
                else
                {
                    string p = keyValueArray[i];
                    map.Add(s, p);
                }
            }

            if (map.Keys.Count == 7)
            {
                this.OwnerId = map[OwnerKey];
                this.AppId = map[AppKey];
                this.ViewerId = map[ViewerKey];
                this.Domain = map[DomainKey];
                
                // NOTE/TODO: appUrl hash is expected here
                // TODO match hash of url
                // AppUrl = map[AppUrlKey]; 
                this.ModuleId = map[ModuleKey];
                this.Container = map[ContainerKey];
            }
        }
    }
}
