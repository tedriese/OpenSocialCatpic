using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Catpic.Host.Engine.Inline
{
    public class CustomHttpRequest : System.Web.HttpRequestBase
    {
        private readonly NameValueCollection _queryString;
        public CustomHttpRequest(NameValueCollection queryString)
        {
            _queryString = queryString;
        }

        public override NameValueCollection QueryString
        {
            get
            {
                return _queryString;
            }
        }

        public override string HttpMethod
        {
            get
            {
                return "GET";
            }
        }

        public override NameValueCollection Params
        {
            get { return _queryString; }
        }
    }
}