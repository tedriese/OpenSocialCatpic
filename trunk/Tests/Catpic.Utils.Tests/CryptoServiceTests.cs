using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catpic.Utils.OAuth;
using NUnit.Framework;

namespace Catpic.Utils.Tests
{
    [TestFixture]
    public class CryptoServiceTests
    {
        [Test]
        public void CanEncryptDecryptData()
        {
            string testStr = @"o:john.doe:a:~/content/gadgets/oauth/oauth.xml:v::d::u::m::c:";
            ICryptoService cryptoService = new AESCryptoService("mysecret");

            var encryptedBytes = cryptoService.Encrypt(testStr);

            ICryptoService cryptoService2 = new AESCryptoService("mysecret");
            var decryptedStr = cryptoService2.Decrypt(encryptedBytes);
            Assert.AreEqual(testStr, decryptedStr);
        }
    }
}
