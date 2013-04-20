using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Catpic.Host.Engine.Inline
{
    public class CustomHttpResponse : System.Web.HttpResponseBase
    {
        private TextWriter _output;
        public CustomHttpResponse(TextWriter output)
        {
            _output = output;
        }

        public override TextWriter Output
        {
            get
            {
                return _output;
            }
            set
            {
                _output = value;
            }
        }
    }
}