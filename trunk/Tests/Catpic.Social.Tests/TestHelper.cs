using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Catpic.Gadgets.Security;
using Catpic.Host.Engine.Social;
using Catpic.Social.Activities;
using Catpic.Social.Formatting;
using Catpic.Social.People;
using Catpic.Utils.Caching;
using Catpic.Utils.OAuth;
using Catpic.Web;
using Catpic.Web.Controllers;
using Newtonsoft.Json;
using NUnit.Framework;
using Rhino.Mocks;

namespace Catpic.Social.Tests
{
    using Catpic.Social.Groups;
    using Catpic.Social.Messages;
    using Catpic.Web.Rules;

    public static class TestHelper
    {
        public static HttpContextBase GetContext(HttpRequestBase request, HttpResponseBase response)
        {
            var context = MockRepository.GenerateMock<HttpContextBase>();
            context.Stub(c => c.Request).Return(request);
            context.Stub(c => c.Response).Return(response);
            context.Stub(c => c.User).Return(new GenericPrincipal(new GenericIdentity("john.doe"), new string[] { }));
            return context;
        }


        public static void AsertResult(string query, string expectedResult)
        {
            //Assign
            var task = TestHelper.GetRpcResult(query);
            task.Wait();
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            var result = JsonConvert.SerializeObject(task.Result, settings);
            Assert.IsNotNullOrEmpty(result);
            Assert.AreEqual(expectedResult, result);
        }

        public static void AssertResult(IEnumerable<RequestItem> requestItems, RpcController controller, string expectedResult)
        {
            var task = controller.PostAsync(requestItems);
            task.Wait();

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            var result = JsonConvert.SerializeObject(task.Result, settings);

            Assert.IsNotNullOrEmpty(result);
            Assert.AreEqual(expectedResult, result);
        }

        public static Task<IEnumerable<object>> GetRpcResult(string query)
        {
            var requestItems = GetRequestItems(query);
            return GetController().PostAsync(requestItems);
        }

        public static IEnumerable<RequestItem> GetRequestItems(string query)
        {
            SocialTypeLocator locator = (new SocialTypeLocator())
               .Register("people", typeof(PersonItem<Person>))
               .Register("activities", typeof(ActivityItem<Activity>))
               .Register("activitystreams", typeof(ActivityItem<ActivityEntry>))
               .Register("messages", typeof(MessageItem<Message>))
               .Register("groups", typeof(GroupItem<Group>));
                JsonRpcFormatter formatter = new JsonRpcFormatter(locator);

                MemoryStream stream = new MemoryStream(Encoding.Default.GetBytes(query));
                var task = formatter.ReadFromStreamAsync(typeof(IEnumerable<RequestItem>), stream, null, null);
                task.Wait();

            return task.Result as IEnumerable<RequestItem>;
        }

        public static RpcController GetController()
        {
            ICryptoService cryptoService = new AESCryptoService("mysecret");
            ICache cache = new RuntimeMemoryCache();
            SecurityTokenFactory tokenFactory = new SecurityTokenFactory(cryptoService, c => cache);

            var loader = new CanonicalDbLoader("canonicaldb.json");

            IRepository<Person> peopleRepository = new PeopleRepository(loader.PeopleCollections);
            var peopleExpressionFactory = new SocialExpressionFactory<Person>();

            IRepository<Activity> activityRepository = new ActivityRepository(loader.ActivityCollections);
            var activityExpressionFactory = new SocialExpressionFactory<Activity>();


            IRepository<ActivityEntry> activityStreamRepository = new ActivityStreamsRepository(loader.ActivityStreamCollections);
            var activityStreamExpressionFactory = new SocialExpressionFactory<ActivityEntry>();

           IRepository<Message> messagesRepository= new MessageRepository(loader.MessageCollections);
           var messageExpressionFactory = new SocialExpressionFactory<Message>();

           IRepository<Group> groupRepository = new GroupRepository(loader.GroupCollections);
           var groupExpressionFactory = new SocialExpressionFactory<Group>();

           IRuleChain ruleChain = (new RuleChain())
               .AddRule(new UserIdRule());
                //.AddRule(new MessageRule());

            // register all services
            IEnumerable<SocialHandler> handlers = new List<SocialHandler>()
                {
                    new PeopleHandler<Person>("people", peopleRepository, peopleExpressionFactory),
                    new ActivityHandler<Activity>("activities", activityRepository, activityExpressionFactory),
                    new ActivityHandler<ActivityEntry>("activitystreams", activityStreamRepository, activityStreamExpressionFactory),
                    new MessageHandler<Message>("messages", messagesRepository, messageExpressionFactory),
                    new GroupHandler<Group>("groups", groupRepository, groupExpressionFactory),
                };

            return new RpcController(tokenFactory, ruleChain, handlers);
        }

        public static void DoubleCheck(string query1, string expectedResult1, string query2, string expectedResult2)
        {
            var requestItems1 = TestHelper.GetRequestItems(query1);
            var requestItems2 = TestHelper.GetRequestItems(query2);

            var controller = TestHelper.GetController();

            TestHelper.AssertResult(requestItems1, controller, expectedResult1);
            TestHelper.AssertResult(requestItems2, controller, expectedResult2);
        }
    }
}
