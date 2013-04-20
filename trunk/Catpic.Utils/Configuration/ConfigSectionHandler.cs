using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Catpic.Utils.Configuration
{
    public class ConfigSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            using (var reader = new XmlNodeReader(section))
                return XElement.Load(reader);
        }
    }
}
