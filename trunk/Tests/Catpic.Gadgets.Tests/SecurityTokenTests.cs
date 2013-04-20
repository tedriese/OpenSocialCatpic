using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Catpic.Gadgets.Security;
using Catpic.Utils.Caching;
using Catpic.Utils.OAuth;
using NUnit.Framework;

namespace Catpic.Gadgets.Tests
{
    [TestFixture]
    public class SecurityTokenTests
    {
        [Test]
        public void CanCreateBaseToken()
        {
            //assign
            ICryptoService cryptoService = new AESCryptoService("mysecret");
            ICache cache = new RuntimeMemoryCache();
            SecurityTokenFactory tokenFactory = new SecurityTokenFactory(cryptoService, c => cache);
            var request = TestHelper.GetRequest("http://catpic.apphb.com");
            var response = TestHelper.GetResponse(new StringWriter());
            var context = TestHelper.GetContext(request, response);

            //act
            var token = tokenFactory.Create(context);

            //token john.doe:john.doe:appid:cont:url:0:default
            Assert.IsNotNull(token, "token");
            Assert.IsAssignableFrom(typeof(BasicSecurityToken), token);
            Assert.AreEqual("john.doe", token.OwnerId);
            Assert.AreEqual("john.doe", token.ViewerId);
        }

        [Test]
        public void CanRSAEncryptDecryptToken()
        {
            ICryptoService cryptoService = new AESCryptoService("mysecret");
            ICache cache = new RuntimeMemoryCache();
            SecurityTokenFactory tokenFactory = new SecurityTokenFactory(cryptoService, c => cache);
            var request = TestHelper.GetRequest("http://catpic.apphb.com");
            var response = TestHelper.GetResponse(new StringWriter());
            var context = TestHelper.GetContext(request, response);

            //act
            var token = tokenFactory.Create(context);

            var state = token.ToClientState();
            Assert.IsNotNullOrEmpty(state);

            BasicSecurityToken emptyToken = new BasicSecurityToken("", "", "", "", "", "", "", cryptoService);
            emptyToken.FromClientState(state);

            Assert.AreEqual(token.OwnerId, emptyToken.OwnerId);
            Assert.AreEqual(token.ViewerId, emptyToken.ViewerId);
        }
    }
}
