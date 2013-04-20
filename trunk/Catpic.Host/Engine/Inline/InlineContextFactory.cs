using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using Catpic.Gadgets;

namespace Catpic.Host.Engine.Inline
{
    /// <summary>
    /// Creates gadget context for inline rendering mode
    /// </summary>
    public class InlineContextFactory
    {
        private readonly IContextFactory _contextFactory;

        public InlineContextFactory(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public GadgetContext Create(NameValueCollection query, TextWriter output, HttpContextBase original)
        {
            var request = new CustomHttpRequest(query);
            var response = new CustomHttpResponse(output);
            var context = new CustomHttpContextBase(original, request, response);
            return _contextFactory.CreateGadgetContext(context);
        }
    }
}