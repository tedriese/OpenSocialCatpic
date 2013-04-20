using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Catpic.Host.Engine.Inline
{
    public class CustomHttpContextBase: HttpContextBase
    {
        private readonly HttpContextBase _context;
        private readonly HttpRequestBase _request;
        private readonly HttpResponseBase _response;

        public CustomHttpContextBase(HttpContextBase original, HttpRequestBase request, HttpResponseBase response)
        {
            _context = original;
            _request = request;
            _response = response;
        }

        public override System.Security.Principal.IPrincipal User
        {
            get
            {
                return _context.User;
            }
            set
            {
                _context.User = value;
            }
        }


        public override HttpRequestBase Request
        {
            get { return _request; }
        }

        public override HttpResponseBase Response
        {
            get { return _response; }
        }
    }
}