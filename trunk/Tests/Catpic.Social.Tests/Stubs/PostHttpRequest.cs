using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace Catpic.Social.Tests.Stubs
{
    public class PostHttpRequest : HttpRequestBase
    {
        private readonly string _body;

        private readonly NameValueCollection _query = new NameValueCollection()
                                         {
                                             {"st","john.doe:john.doe:appid:cont:url:0:default"},
                                         };

        public PostHttpRequest(string body)
        {
            _body = body;
        }

        public override byte[] BinaryRead(int count)
        {
            return Encoding.Default.GetBytes(_body);
            // return Encoding.Default.GetBytes("[{\"method\":\"people.get\",\"params\":{\"userId\":[\"@viewer\"],\"groupId\":\"@friends\",\"networkDistance\":1,\"filterBy\":\"displayName\",\"filterValue\":\"Name1\",\"sortBy\":\"thumbnail\",\"sortOrder\":\"ascending\", \"fields\":[\"id\",\"displayName\"]}}]");
        }

        public override NameValueCollection QueryString
        {
            get { return _query; }
        }

        public override NameValueCollection Params
        {
            get
            {
                return _query;
            }
        }

        public override string HttpMethod
        {
            get { return "POST"; }
        }

        public override int ContentLength
        {
            get
            {
                return 13;
            }
        }
    }
}
