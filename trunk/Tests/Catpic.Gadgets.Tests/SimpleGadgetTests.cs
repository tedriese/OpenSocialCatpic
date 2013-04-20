using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Catpic.Gadgets.Format;
using Catpic.Gadgets.Security;
using Catpic.Utils.Caching;
using Catpic.Utils.OAuth;
using NUnit.Framework;

namespace Catpic.Gadgets.Tests
{
    /// <summary>
    /// Tests the simplest gadgets
    /// </summary>
    [TestFixture]
    public class SimpleGadgetTests
    {

      /*[Test]
        public void CanFetchAndCreateHoroscopeGadget()
        {
            //assign
            StringWriter writer = new StringWriter();
            var task = GetCreateGadgetTask(TestConsts.RemoteHoroscopeGadget, "iframe" ,writer);
            //task.Start();
            task.Wait();
            Assert.IsNotNullOrEmpty(writer.ToString());
        }*/


        [Test]
        public void CanFetchAndCreateIframedTestGadget()
        {
           //assign
            StringWriter writer = new StringWriter();
            var task = TestHelper.GetCreateGadgetTask(Path.GetFullPath(TestConsts.TestGadget), "iframe", writer);
            task.Wait();
            var output = writer.ToString();
            Assert.IsNotNullOrEmpty(output);
            //NOTE: improve approach of checking
            Assert.IsTrue(output.Contains("<p>TEST!!!</p>"));
           // Assert.IsTrue(output.Contains("concat?")); //contain concat
            Assert.IsTrue(output.Contains("<script>")); //has at least one script
            Assert.IsFalse(output.Contains("<div>")); //isn't inline div (begin)
            Assert.IsFalse(output.Contains("</div>")); //isn't  inline div (end)
            Assert.IsTrue(output.Contains("<html><head>")); //is html (begin)
            Assert.IsTrue(output.Contains("</body></html>"));//is html (end)
        }

        [Test]
        public void CanFetchAndCreateInlinedTestGadget()
        {
            //assign
            StringWriter writer = new StringWriter();
            var task = TestHelper.GetCreateGadgetTask(Path.GetFullPath(TestConsts.TestGadget), "inline", writer);
            task.Wait();
            var output = writer.ToString();
            Assert.IsNotNullOrEmpty(output);
            //NOTE: improve approach of checking
            Assert.IsTrue(output.Contains("<p>TEST!!!</p>"));
           // Assert.IsTrue(output.Contains("concat?")); //contain concat
            Assert.IsTrue(output.Contains("<script>")); //has at least one script
            Assert.IsTrue(output.Contains("<div>")); //is inline div (begin)
            Assert.IsTrue(output.Contains("</div>")); //is inline div (end)
            Assert.IsFalse(output.Contains("<html><head>")); //isn't html (begin)
            Assert.IsFalse(output.Contains("</body></html>"));//isn't html (end)
        }
        
        [Test]
        public void CanParseHoroscopeGadget()
        {
            var http = TestHelper.GetContext(TestHelper.GetRequest(TestConsts.RemoteHoroscopeGadget), TestHelper.GetResponse(new StringWriter()));
            ICache cache = new RuntimeMemoryCache();
            ICryptoService cryptoService = new AESCryptoService("mysecret");
            SecurityTokenFactory tokenFactory = new SecurityTokenFactory(cryptoService, c => cache); ;
            var token = tokenFactory.Create(http);
            var context = new GadgetContext(http, token);
            GadgetParser parser = new GadgetParser();

            var xdocGadget = XDocument.Load(TestConsts.LocalHoroscopeGadget);
            var definition = parser.Parse(xdocGadget, context.Uri);
            Assert.IsNotNull(definition, "GadgetDefinition");
            Assert.IsNotNull(definition.ModulePreferences, "ModulePreferences");
            Assert.IsNotNull(definition.UserPreferences, "UserPreferences");

            Assert.AreEqual(74, definition.ModulePreferences.Locales.Count(), "Locales count");
            Assert.AreEqual(5, definition.ModulePreferences.RequiredFeatures.Count(), "Features count");
            Assert.AreEqual(5, definition.UserPreferences.Count(), "User preferences count");
            Assert.AreEqual(4, definition.Views.Count(), "Views count");
        }
    }
}
