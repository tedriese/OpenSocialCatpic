using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Catpic.Gadgets.Format;
using Catpic.Gadgets.Security;
using Catpic.Gadgets.Security.OAuth;
using Catpic.Utils.Caching;
using Catpic.Utils.Configuration;
using Catpic.Utils.OAuth;
using NUnit.Framework;

namespace Catpic.Gadgets.Tests
{
    [TestFixture]
    public class OAuthTests
    {
        [Test]
        public void CanParseOauthSection()
        {
            //fake url is used
            ICryptoService cryptoService = new AESCryptoService("mysecret");
            ICache cache = new RuntimeMemoryCache();
            SecurityTokenFactory tokenFactory = new SecurityTokenFactory(cryptoService, c => cache);
            var http = TestHelper.GetContext(TestHelper.GetRequest("http://localhost"),
                                             TestHelper.GetResponse(new StringWriter()));
            var token = tokenFactory.Create(http);
            var context = new GadgetContext(http, token);
            GadgetParser parser = new GadgetParser();

            var xdocGadget = XDocument.Load(TestConsts.OAuthGadget);
            var definition = parser.Parse(xdocGadget, context.Uri);
            var oauth = definition.ModulePreferences.OAuth;
            Assert.IsNotNull(oauth, "OAuth");
            Assert.IsNotNull(oauth.Services, "Services");

            Assert.AreEqual(oauth.Services.Count(), 1);
            var service = oauth.Services.ToArray()[0];
            Assert.AreEqual(service.Request.Endpoint.ToString(), "http://localhost:9999/oauth-provider/request_token");
            Assert.AreEqual(service.Access.Endpoint.ToString(), "http://localhost:9999/oauth-provider/access_token");
            Assert.AreEqual(service.Authorization.ToString(), "http://localhost:9999/oauth-provider/authorize?oauth_callback=http://localhost:9999/gadgets/oauthcallback");
        }

        [Test]
        public void CanProcessOAuthStorage()
        {
            var configs = ConfigSettings.Instance.GetSections("oauth/services/service");
            List<OAuthConsumer> consumers = new List<OAuthConsumer>();
            foreach (var configSection in configs)
            {
                OAuthConsumer consumer = new OAuthConsumer();
                consumer.AppId = configSection.GetString("@appId");
                consumer.Service = configSection.GetString("@name");
                consumer.KeyType = configSection.GetString("consumer/type");
                consumer.Key = configSection.GetString("consumer/key");
                consumer.Secret = configSection.GetString("consumer/secret");
                consumers.Add(consumer);
            }

            Assert.AreEqual(1, consumers.Count);
            var testConsumer = consumers.Single(c => c.AppId == "~/content/gadgets/oauth/oauth.xml");
            Assert.AreEqual("~/content/gadgets/oauth/oauth.xml", testConsumer.AppId );
            Assert.AreEqual("oauth-sandbox", testConsumer.Service);
            Assert.AreEqual("5f3c36b7ef5cd3ac", testConsumer.Key);
            Assert.AreEqual("964709b9ab578e7b3d12fdde72d2", testConsumer.Secret);
            Assert.AreEqual("HMAC-SHA1", testConsumer.KeyType);
        }
    }
}
