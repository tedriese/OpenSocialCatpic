namespace Catpic.Gadgets.Format
{
    using System;
    using System.Xml.Linq;

    /// <summary>
    /// Parses gadget
    /// </summary>
    public class GadgetParser : IGadgetParser
    {
        /// <summary>
        /// Module preference parser.
        /// </summary>
        private readonly ModulePreferencesParser _modulePrefsParser;

        /// <summary>
        /// User preferences parser
        /// </summary>
        private readonly UserPreferencesParser _userPrefPars;

        /// <summary>
        /// Content parser
        /// </summary>
        private readonly ContentParser _contentParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="GadgetParser"/> class.
        /// </summary>
        public GadgetParser()
        {
            this._modulePrefsParser = new ModulePreferencesParser();
            this._userPrefPars = new UserPreferencesParser();
            this._contentParser = new ContentParser();
        }

        /// <summary>
        /// Parse gadget definition from xdocument
        /// </summary>
        /// <param name="xdocGadget"> The xdoc gadget. </param>
        /// <param name="baseUri"> The base uri. </param>
        /// <returns> Gadget definition </returns>
        public GadgetDefinition Parse(XDocument xdocGadget, Uri baseUri)
        {
            // TODO assert xml doc
            // parse modulePrefs
            var xModulePrefs = xdocGadget.Root.Element("ModulePrefs");
            var modulePrefs = this._modulePrefsParser.Parse(xModulePrefs, baseUri);
            var userPreferences = this._userPrefPars.Parse(xdocGadget.Root);
            var views = this._contentParser.Parse(xdocGadget.Root);

            return new GadgetDefinition(modulePrefs, userPreferences, views);
        }
    }
}
