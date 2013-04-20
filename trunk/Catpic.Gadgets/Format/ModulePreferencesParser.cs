// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModulePreferencesParser.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Creates ModulePreferences object from xml node
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Format
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    using Catpic.Gadgets.Format.OAuth;
    using Catpic.Gadgets.Format.OAuth2;

    /// <summary>
    /// Creates ModulePreferences object from xml node
    /// </summary>
    public class ModulePreferencesParser
    {
        /// <summary>
        /// Parses module preferences.
        /// </summary>
        /// <param name="xModulePref"> The x module pref. </param>
        /// <param name="baseUri"> The base uri. </param>
        /// <returns> Module preferences object. </returns>
        public virtual ModulePreferences Parse(XElement xModulePref, Uri baseUri)
        {
            // TODO icon
            var modulePrefs = new ModulePreferences
                {
                    Header = this.ParseHeader(xModulePref),
                    RequiredFeatures = this.ParseFeatures(xModulePref, "Require"),
                    OptionalFeatures = this.ParseFeatures(xModulePref, "Optional"),
                    Locales = this.ParseLocales(xModulePref, baseUri).ToList(),
                    Preloads = this.ParsePreloads(xModulePref),
                    OAuth = this.ParseOAuth(xModulePref),
                    OAuth2 = this.ParseOAuth2(xModulePref),
                    Icon = this.ParseIcon(xModulePref)
                };

            return modulePrefs;
        }

        #region Protected methods

        /// <summary>
        /// Reads all modulePrefs node's attributes into dictionary
        /// </summary>
        /// <param name="xModulePref"> The x module pref. </param>
        /// <returns> Header attributes map</returns>
        protected virtual IDictionary<string, string> ParseHeader(XElement xModulePref)
        {
            var attributes = new Dictionary<string, string>();
            foreach (var attribute in xModulePref.Attributes())
            {
                attributes.Add(attribute.Name.LocalName, attribute.Value);
            }

            return attributes;
        }

        /// <summary>
        /// Parse all modulePrefs' locales
        /// </summary>
        /// <param name="xModulePrefs"> The x module pref. </param>
        /// <param name="baseUri">Gadget uri</param>
        /// <returns>The list of locale definitions</returns>
        protected virtual IEnumerable<LocaleDefinition> ParseLocales(XElement xModulePrefs, Uri baseUri)
        {
            foreach (var xLocale in xModulePrefs.Elements("Locale"))
            {
                var locale = new LocaleDefinition();
                var lang = xLocale.Attribute("lang");

                locale.Language = lang == null ? "all" : lang.Value;
                var messages = xLocale.Attribute("messages");
                if (messages != null)
                {
                    var messagesUri = new Uri(messages.Value, UriKind.RelativeOrAbsolute);
                    if (!messagesUri.IsAbsoluteUri)
                    {
                        messagesUri = new Uri(baseUri, messagesUri);
                    }

                    locale.Messages = new MessageBundle(messagesUri);
                }
                else
                {
                    // TODO get messages node
                    // XElement msgNode = null;
                    // locale.Messages = new MessageBundle(msgNode);
                }

                yield return locale;
            }
        }

        /// <summary>
        /// Processes OAuth section
        /// </summary>
        /// <param name="xModulePrefs"> The x module pref. </param>
        /// <returns> Oauth1 definition</returns>
        protected virtual OAuthDefinition ParseOAuth(XElement xModulePrefs)
        {
            var xOauth = xModulePrefs.Element("OAuth");

            if (xOauth != null)
            {
                var services = new List<ServiceDefinition>();
                
                // NOTE OAuth 1.0 settings expected
                foreach (var xService in xOauth.Elements("Service"))
                {
                    var service = new ServiceDefinition();
                    try
                    {
                        var name = xService.Attribute("name");
                        if (name != null)
                        {
                            service.Name = name.Value;
                        }

                        service.Request = this.ParseTokenRequest(xService.Element("Request"));
                        service.Access = this.ParseTokenRequest(xService.Element("Access"));
                        service.Authorization = new Uri(xService.Element("Authorization").Attribute("url").Value);

                        services.Add(service);
                    }
                    catch (Exception ex)
                    {
                        // TODO
                    }
                }

                var oauth = new OAuthDefinition { Services = services };
                return oauth;
            }

            return null;
        }

        /// <summary>
        /// Processes OAuth section
        /// </summary>
        /// <param name="xModulePrefs"> The x module pref. </param>
        /// <returns>Oauth2 definition.</returns>
        protected virtual OAuth2Definition ParseOAuth2(XElement xModulePrefs)
        {
            var xOauth = xModulePrefs.Element("OAuth2");

            if (xOauth != null)
            {
                var services = new List<Service2Definition>();
                
                // NOTE OAuth2 settings expected
                foreach (var xService in xOauth.Elements("Service"))
                {
                    var service = new Service2Definition();
                    try
                    {
                        var name = xService.Attribute("name");
                        if (name != null)
                        {
                            service.Name = name.Value;
                        }

                        var scrope = xService.Attribute("scope");
                        if (scrope != null)
                        {
                            service.Scope = scrope.Value;
                        }

                        var xAuth = xService.Element("Authorization");
                        if (xAuth != null)
                        {
                            service.Authorization = this.ParseTokenRequest(xAuth);
                        }

                        var xAccess = xService.Element("Access");
                        if (xAccess != null)
                        {
                            service.Access = this.ParseTokenRequest(xAccess);
                        }

                        services.Add(service);
                    }
                    catch (Exception ex)
                    {
                        // TODO
                    }
                }

                var oauth = new OAuth2Definition { Services = services };
                return oauth;
            }

            return null;
        }

        /// <summary>
        /// Processes features
        /// </summary>
        /// <param name="xModulePref"> The x module pref. </param>
        /// <param name="nodeName">xml node name</param>
        /// <returns> The list of features</returns>
        protected virtual IEnumerable<Feature> ParseFeatures(XElement xModulePref, string nodeName)
        {
            // TODO separate feature definition from this
            foreach (var xFeature in xModulePref.Elements(nodeName))
            {
                var feature = new Feature();
                feature.Name = xFeature.Attribute("feature").Value;

                // initialize parameters
                var paramMap = new Dictionary<string, string>();
                var xParams = xFeature.Elements("Param");
                foreach (var xParam in xParams)
                {
                    var name = xParam.Attribute("name").Value;
                    var value = xParam.Value;
                    paramMap.Add(name, value);
                }

                feature.Parameteres = paramMap;
                yield return feature;
            }
        }

        /// <summary>
        /// Parses preload sections.
        /// </summary>
        /// <param name="xModulePref"> The x module pref. </param>
        /// <returns> Preload collection.</returns>
        protected virtual IEnumerable<PreloadDefinition> ParsePreloads(XElement xModulePref)
        {
            foreach (var xPreload in xModulePref.Elements("Preload"))
            {
                var preload = new PreloadDefinition();
                preload.Href = new Uri(xPreload.Attribute("href").Value);

                // TODO: initialize authz properties
                yield return preload;
            }
        }

        /// <summary>
        /// Parses module icon
        /// </summary>
        /// <param name="xModulePref"> The x module pref. </param>
        /// <returns> Icon definition </returns>
        protected virtual IconDefinition ParseIcon(XElement xModulePref)
        {
            // NOTE: Deprecated Use /ModulePrefs/Link/@rel="icon" instead.
            // TODO
            return null;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets token request information.
        /// </summary>
        /// <param name="xToken">Token xml.</param>
        /// <returns>Token request section.</returns>
        private TokenRequest ParseTokenRequest(XElement xToken)
        {
            var token = new TokenRequest();

            token.Endpoint = new Uri(xToken.Attribute("url").Value);

            var method = xToken.Attribute("method");
            token.Method = method != null ? method.Value : "POST";

            var paramLocation = xToken.Attribute("param_location");
            token.ParamLocation = paramLocation != null ? paramLocation.Value : "auth-header";

            return token;
        }

        #endregion
    }
}
