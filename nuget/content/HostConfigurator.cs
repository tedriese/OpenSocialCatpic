using System.Web;
using Catpic.Social.Activities;
using Catpic.Social.People;
using Catpic.Utils;
using Catpic.Web.Configuration;
using Microsoft.Practices.Unity;

namespace YourApp
{
    using Catpic.Host.Engine.Social;
    using Catpic.Social;
    using Catpic.Social.Groups;
    using Catpic.Social.Messages;

    /// <summary>
    /// Configures host to use default catpic settings
    /// </summary>
    public static class HostConfigurator
    {
        public static void Configure(IUnityContainer container)
        {
            //NOTE: need to configure callback url, but unable to receive host url in Application_Start event in production
            var serverPath = "http://yourserver/gadgets/oauth_callback";
            try
            {
                serverPath = string.Format(@"{0}://{1}/gadgets/oauth_callback", HttpContext.Current.Request.Url.Scheme,
                                           HttpContext.Current.Request.Url.Authority);
            }
            catch { }

            //Set trace: you can set your traces by implementing of catpic's ITrace interface;
			//see Catpic.Host project for details
            //CatpicConfigurator.ConfigureTrace(new Log4NetTrace());
			
			//This is short demo which uses modified shindig's canonicaldb database
            var loader = new CanonicalDbLoader(@"~/App_Data/canonicaldb.json");
            
			//register social repositories before configure all services
            container.RegisterInstance(typeof(IRepository<Person>), new PeopleRepository(loader.PeopleCollections));
            container.RegisterInstance(typeof(IRepository<Activity>), new ActivityRepository(loader.ActivityCollections));
            container.RegisterInstance(typeof(IRepository<ActivityEntry>), new ActivityStreamsRepository(loader.ActivityStreamCollections));
            container.RegisterInstance(typeof(IRepository<Message>), new MessageRepository(loader.MessageCollections));
            container.RegisterInstance(typeof(IRepository<Group>), new GroupRepository(loader.GroupCollections));

            container.RegisterType(typeof(SocialExpressionFactory<Person>), typeof(SocialExpressionFactory<Person>));
            container.RegisterType(typeof(SocialExpressionFactory<Activity>), typeof(SocialExpressionFactory<Activity>));
            container.RegisterType(typeof(SocialExpressionFactory<ActivityEntry>), typeof(SocialExpressionFactory<ActivityEntry>));
            container.RegisterType(typeof(SocialExpressionFactory<Message>), typeof(SocialExpressionFactory<Message>));
            container.RegisterType(typeof(SocialExpressionFactory<Group>), typeof(SocialExpressionFactory<Group>));
			
            //Catpic default configuration forces you to use dependency injection container
			//In this example, unity is used. However catpic hasn't dependency on certain DI container implementation
            CatpicConfigurator.ConfigureServices(serverPath, "mysecret", new UnityHostContainer(container));
        }
    }	
}