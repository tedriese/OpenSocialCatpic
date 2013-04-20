// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GadgetDefinitionFactory.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Produces gadget definition
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Xml.Linq;

    using Catpic.Gadgets.Format;
    using Catpic.Utils.Caching;

    /// <summary>
    /// Produces gadget definition
    /// </summary>
    public class GadgetDefinitionFactory : IGadgetDefinitionFactory
    {
        /// <summary>
        /// Cache name for gadget definitions
        /// </summary>
        private const string CacheNamespace = "gadget";

        /// <summary>
        /// Gadget parser
        /// </summary>
        private readonly GadgetParser _gadgetParser;

        /// <summary>
        /// Cache instance
        /// </summary>
        private readonly ICache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="GadgetDefinitionFactory"/> class.
        /// </summary>
        /// <param name="gadgetParser"> Gadget parser. </param>
        /// <param name="cacheResolver"> Cache resolver. </param>
        public GadgetDefinitionFactory(GadgetParser gadgetParser, Func<string, ICache> cacheResolver)
        {
           this._gadgetParser = gadgetParser;
           this._cache = cacheResolver(CacheNamespace);
        }

        /// <summary>
        /// Creates gadget definition from response.
        /// </summary>
        /// <param name="uri"> Gadget uri. </param>
        /// <param name="response"> Web response contains raw gadget definition. </param>
        /// <returns> GadgetDefinition instance.</returns>
        public GadgetDefinition Create(Uri uri, WebResponse response)
        {
            GadgetDefinition gadgetDefinition = null;

            // prevent encoding problems in some gadgets
            Encoding encoding = Encoding.Default;
            if (response is HttpWebResponse)
            {
                var charset = (response as HttpWebResponse).CharacterSet;
                if (!string.IsNullOrEmpty(charset))
                {
                    encoding = Encoding.GetEncoding(charset);
                }
            }

            using (var gadgetStream = new StreamReader(response.GetResponseStream(), encoding))
            {
                XDocument xdocGadget = XDocument.Load(gadgetStream);
                gadgetDefinition = this._gadgetParser.Parse(xdocGadget, uri);
                if (this._cache.Contains(uri))
                {
                    // discard previous calculation
                    return this._cache.Get(uri) as GadgetDefinition; 
                }

                this._cache.Add(uri, gadgetDefinition);
            }

            return gadgetDefinition;
        }

        /// <summary>
        /// Returns gadget definition instance
        /// </summary>
        /// <param name="uri"> Gadget uri. </param>
        /// <returns>
        /// Gadget Definition from cache or null
        /// </returns>
        public GadgetDefinition Get(Uri uri)
        {
            if (this._cache.Contains(uri))
            {
                return this._cache.Get(uri) as GadgetDefinition;
            }

            return null;
        }
    }
}
