using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Catpic.Social.Tests.Stubs
{
    public class StubHttpResponse : HttpResponseBase
    {
        private readonly TextWriter _writer;
        public StubHttpResponse(TextWriter writer)
        {
            _writer = writer;
        }

        public override string ContentType { get; set; }

        public override TextWriter Output
        {
            get
            {
                return _writer;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
