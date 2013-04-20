// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatpicConfigurator.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Configures catpic host to follow "Code over Configuration" principle
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Web.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http.Formatting;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Xml.Linq;

    using Catpic.Gadgets;
    using Catpic.Gadgets.Containers;
    using Catpic.Gadgets.Format;
    using Catpic.Gadgets.Proxies;
    using Catpic.Gadgets.Rendering.Container;
    using Catpic.Gadgets.Rendering.Gadget;
    using Catpic.Gadgets.Security;
    using Catpic.Gadgets.Security.OAuth;
    using Catpic.Gadgets.Security.OAuth2;
    using Catpic.Social;
    using Catpic.Social.Activities;
    using Catpic.Social.Formatting;
    using Catpic.Social.Groups;
    using Catpic.Social.Messages;
    using Catpic.Social.People;
    using Catpic.Utils;
    using Catpic.Utils.Caching;
    using Catpic.Utils.Configuration;
    using Catpic.Utils.Diagnostic;
    using Catpic.Utils.OAuth;
    using Catpic.Web.Rules;

    /// <summary>
    /// Configures catpic host to follow "Code over Configuration" principle
    /// </summary>
    public static class CatpicConfigurator
    {
        /// <summary>
        /// The list of caches
        /// </summary>
        private static readonly IDictionary<string, ICache> Caches = new Dictionary<string, ICache>
            {
                { "default", new RuntimeMemoryCache() },
                { "tokens", new RuntimeMemoryCache() },
                { "preload", new RuntimeMemoryCache() },
                { "metadata", new RuntimeMemoryCache() },

                // used for storing of gadget definitions
                { "gadget", new RuntimeMemoryCache() },
            };

        /// <summary>
        /// Base server path used for callbacks
        /// </summary>
        private static string _serverPath;

        /// <summary>
        /// Encryption key
        /// </summary>
        private static string _secret;

        /// <summary>
        /// Container host
        /// </summary>
        private static IHostContainer _hostContainer;

        #region Public members

        /// <summary>
        /// Configures trace
        /// </summary>
        /// <param name="trace">Trace instance</param>
        public static void ConfigureTrace(ITrace trace)
        {
            TraceFactory.SetTrace(trace);
        }

        /// <summary>
        /// Configures Web API formatting
        /// </summary>
        /// <param name="config"> WebAPI configuration. </param>
        /// <param name="routes"> MVC routes. </param>
        public static void ConfigureRoutes(HttpConfiguration config, RouteCollection routes)
        {
            // Rpc webApi routing
            routes.MapHttpRoute("rpc", "gadgets/rpc", new { id = RouteParameter.Optional, controller = "Rpc" });

            routes.MapHttpRoute(
                "messaging.write",
                "rest/messages/{userId}/@self/{messageCollectionId}/{messageId}",
                new
                    {
                        controller = "Messages",
                        messageCollectionId = UrlParameter.Optional,
                        messageId = UrlParameter.Optional
                    });

            routes.MapHttpRoute(
               "messaging.read",
               "rest/messages/{userId}/{messageCollectionId}/{messageId}",
               new
               {
                   controller = "Messages",
                   messageCollectionId = UrlParameter.Optional,
                   messageId = UrlParameter.Optional
               });

            routes.MapHttpRoute(
               "social",
               "rest/{controller}/{userId}/{groupId}/{itemId}",
               new
               {
                   groupId = UrlParameter.Optional,
                   itemId = UrlParameter.Optional
               });

            routes.MapHttpRoute("misc", "rest/{controller}/{action}");
        }

        /// <summary>
        /// Configures WebAPI formatters
        /// </summary>
        /// <param name="config"> The config. </param>
        /// <param name="locator"> The locator. </param>
        public static void ConfigureFormatters(HttpConfiguration config, SocialTypeLocator locator = null)
        {
            if (locator == null)
            {
                locator = new SocialTypeLocator();
            }

            locator
                .Register("people", typeof(PersonItem<Person>))
                .Register("activities", typeof(ActivityItem<Activity>))
                .Register("activitystreams", typeof(ActivityItem<ActivityEntry>))
                .Register("groups", typeof(ActivityItem<ActivityEntry>))
                .Register("messages", typeof(MessageItem<Message>));

            config.Formatters.Clear();
            config.Formatters.Add(new JsonRpcFormatter(locator));

            // TODO set xml formatter (need to set KnowType to DTO)
            // config.Formatters.Add(new XmlMediaTypeFormatter());
        }

        /// <summary>
        /// Configures all services by default
        /// </summary>
        /// <param name="serverPath">Server base path</param>
        /// <param name="secret">Secret key for encryption</param>
        /// <param name="hostContainer">Host container implementation</param>
        public static void ConfigureServices(string serverPath, string secret, IHostContainer hostContainer)
        {
            _serverPath = serverPath;
            _secret = secret;
            _hostContainer = hostContainer;

            RegisterBaseTypes();
            RegisterUtils();
            RegisterGadgetServer();
            RegisterOauthServices();
            RegisterSocialServices();
        }

        #endregion

        #region Service registration

        /// <summary>
        /// Registers base types
        /// </summary>
        private static void RegisterBaseTypes()
        {
            if (!_hostContainer.IsRegistered<IContainerProvider>())
            {
                _hostContainer.RegisterType<IContainerProvider, ContainerProvider>();
            }

            // creates feature list
            if (!_hostContainer.IsRegistered<IFeatureProvider>())
            {
                _hostContainer.RegisterType<IFeatureProvider, FeatureProvider>();
            }

            // renders container specific javascript
            if (!_hostContainer.IsRegistered<IContainerRenderPipeline>())
            {
                _hostContainer.RegisterType<IContainerRenderPipeline, ContainerRenderPipeline>();
            }

            // renders gadget content
            if (!_hostContainer.IsRegistered<GadgetRenderPipeline>())
            {
                _hostContainer.RegisterType<GadgetRenderPipeline, GadgetRenderPipeline>();
            }

            // processes gadget xml
            if (!_hostContainer.IsRegistered<GadgetParser>())
            {
                _hostContainer.RegisterType<GadgetParser, GadgetParser>();
            }

            // create gadget definition from response using parser
            if (!_hostContainer.IsRegistered<GadgetDefinitionFactory>())
            {
                _hostContainer.RegisterType<GadgetDefinitionFactory, GadgetDefinitionFactory>();
            }

            // creates security token for given http context
            if (!_hostContainer.IsRegistered<ISecurityTokenFactory>())
            {
                _hostContainer.RegisterType<ISecurityTokenFactory, SecurityTokenFactory>();
            }

            // provides different contexts which are used by different services
            if (!_hostContainer.IsRegistered<IContextFactory>())
            {
                _hostContainer.RegisterType<IContextFactory, ContextFactory>();
            }

            if (!_hostContainer.IsRegistered<ISecurityRequestHandler>())
            {
                _hostContainer.RegisterType<ISecurityRequestHandler, SecurityRequestHandler>();
            }

            // processes oauth consumers
            if (!_hostContainer.IsRegistered<IOAuthConsumerProvider>())
            {
                _hostContainer.RegisterType<IOAuthConsumerProvider, OAuthConsumerProvider>();
            }

            // proxies requests (e.g. makeRequest())
            if (!_hostContainer.IsRegistered<IRequestProxy>())
            {
                _hostContainer.RegisterType<IRequestProxy, RequestProxy>();
            }

            // processes concat requests
            if (!_hostContainer.IsRegistered<IConcatProxy>())
            {
                _hostContainer.RegisterType<IConcatProxy, ConcatProxy>();
            }

            // facade for gadget services
            if (!_hostContainer.IsRegistered<IRequestHandler>())
            {
                _hostContainer.RegisterType<IRequestHandler, RequestHandler>();
            }

            // rule chain
            if (!_hostContainer.IsRegistered<IRuleChain>())
            {
                IRuleChain ruleChain = (new RuleChain())
                                        .AddRule(new UserIdRule())
                                        .AddRule(new MessageRule());

                _hostContainer.RegisterInstance(typeof(IRuleChain), ruleChain);
            }
        }

        /// <summary>
        /// REgister utils services
        /// </summary>
        private static void RegisterUtils()
        {
            // cache
            if (!_hostContainer.IsRegistered<Func<string, ICache>>())
            {
                Func<string, ICache> cacheResolver = name => Caches[name];
                _hostContainer.RegisterInstance(typeof(Func<string, ICache>), cacheResolver);
            }

            // trace
            if (!TraceFactory.IsInitialized)
            {
                TraceFactory.SetTrace(new EmptyTrace());
            }
        }

        /// <summary>
        /// Register gadget services
        /// </summary>
        private static void RegisterGadgetServer()
        {
            // Register gadgets pipeline
            // Order is important!
            if (!_hostContainer.IsRegistered<IGadgetRenderModule>())
            {
                IEnumerable<IGadgetRenderModule> gadgetRenderModules = new List<IGadgetRenderModule>
                    {
                        new ViewGadgetRenderModule(),
                        new FeatureGadgetRenderModule(),
                        new UserPreferencesGadgetRenderModule(),
                        new MessageGadgetRenderModule(),
                        new UtilGadgetRenderModule(Caches["preload"]),
                        new ConcatGadgetRenderModule()
                    };
                _hostContainer.RegisterInstance(typeof(IEnumerable<IGadgetRenderModule>), gadgetRenderModules);
            }

            if (!_hostContainer.IsRegistered<IContainerRenderModule>())
            {
                IEnumerable<IContainerRenderModule> containerRenderModules = new List<IContainerRenderModule>
                    {
                        new FeatureContainerRenderModule(), 
                        new MetadataContainerRenderModule(Caches["metadata"]) 
                    };
                _hostContainer.RegisterInstance(typeof(IEnumerable<IContainerRenderModule>), containerRenderModules);
            }

            if (!_hostContainer.IsRegistered<IContainer>())
            {
                // features
                var defaultFeatureSet = new FeatureSet(
                    "default",
                    "~/catpic/features/features",
                    "features.txt",
                    new List<string> { "~/catpic/features/resources" });

                var extrasFeatureSet = new FeatureSet(
                    "extras",
                    "~/catpic/features/features-extras",
                    "features.txt",
                    new List<string> { "~/catpic/features/resources" });

                _hostContainer.RegisterInstance(
                    typeof(IEnumerable<IFeatureSet>), new[] { defaultFeatureSet, extrasFeatureSet });

                // containers
                var mainEntry = new DefaultContainer.ContainerFeatureEntry
                {
                    FeatureSet = defaultFeatureSet,
                    ContainerFeatures = new List<string> { "shindig-container", "osapi" },
                    GadgetFeatures = new List<string> { "core" },
                };

                var extraEntry = new DefaultContainer.ContainerFeatureEntry
                {
                    FeatureSet = extrasFeatureSet,
                    ContainerFeatures = new List<string>(),
                    GadgetFeatures = new List<string>(),
                };

                var gadgetDefinitionFactory = _hostContainer.Resolve<GadgetDefinitionFactory>();
                var container = new DefaultContainer(
                    "default",
                    "~/catpic/config/container.js",
                    new List<DefaultContainer.ContainerFeatureEntry> { mainEntry, extraEntry },
                    gadgetDefinitionFactory);

                _hostContainer.RegisterInstance(
                    typeof(IEnumerable<IContainer>),
                    new List<IContainer> { container });
            }
        }

        /// <summary>
        /// Register oauth services
        /// </summary>
        private static void RegisterOauthServices()
        {
            _hostContainer.RegisterInstance(typeof(ICryptoService), new AESCryptoService(_secret));
            XElement xOauthConfig =
                XDocument.Load(FileHelper.ResolvePath("~/catpic/config/catpic.oauth.config")).Root;

            if (!_hostContainer.IsRegistered<IEnumerable<IOAuthRequestHandler>>())
            {
                var oauthConfig = new ConfigSection(new ConfigElement(xOauthConfig));
                var configs = oauthConfig.GetSections("oauth/services/service");
                var consumers = new List<OAuthConsumer>();
                foreach (var configSection in configs)
                {
                    var consumer = new OAuthConsumer
                        {
                            AppId = configSection.GetString("@appId"),
                            Service = configSection.GetString("@name"),
                            KeyType = configSection.GetString("consumer/type"),
                            Key = configSection.GetString("consumer/key"),
                            Secret = configSection.GetString("consumer/secret")
                        };
                    consumers.Add(consumer);
                }

                _hostContainer.RegisterInstance(typeof(IEnumerable<OAuthConsumer>), consumers);

                var oauth2Config = new ConfigSection(new ConfigElement(xOauthConfig));
                var configs2 = oauth2Config.GetSections("oauth2/services/service");
                var consumers2 = new List<OAuth2Consumer>();
                foreach (var configSection in configs2)
                {
                    var consumer = new OAuth2Consumer
                        {
                            AppId = configSection.GetString("@appId"),
                            Service = configSection.GetString("@name"),
                            UsesAuthorizationHeader = configSection.GetBool("@usesAuthHeader"),
                            ClientId = configSection.GetString("consumer/clientId"),
                            Secret = configSection.GetString("consumer/secret"),
                            Type = configSection.GetString("consumer/type")
                        };
                    consumers2.Add(consumer);
                }

                _hostContainer.RegisterInstance(typeof(IEnumerable<OAuth2Consumer>), consumers2);

                var gadgetFactory = _hostContainer.Resolve<GadgetDefinitionFactory>();
                var oauthProvider = _hostContainer.Resolve<IOAuthConsumerProvider>();
                var cacheResolver = _hostContainer.Resolve<Func<string, ICache>>();

                _hostContainer.RegisterInstance(
                    typeof(IEnumerable<IOAuthRequestHandler>),
                    new List<IOAuthRequestHandler>
                        {
                            new OAuthRequestHandler(gadgetFactory, oauthProvider, _serverPath, cacheResolver),
                                new OAuth2RequestHandler(gadgetFactory, oauthProvider, _serverPath, cacheResolver)
                    });
            }
        }

        /// <summary>
        /// Register social services
        /// </summary>
        private static void RegisterSocialServices()
        {
            if (!_hostContainer.IsRegistered<IEnumerable<SocialHandler>>())
            {
                // NOTE: repositories should be registered
                var peopleRepository = _hostContainer.Resolve<IRepository<Person>>();
                var activityRepository = _hostContainer.Resolve<IRepository<Activity>>();
                var activityStreamRepository = _hostContainer.Resolve<IRepository<ActivityEntry>>();
                var messagesRepository = _hostContainer.Resolve<IRepository<Message>>();
                var groupsRepository = _hostContainer.Resolve<IRepository<Group>>();

                // create expression factories
                var peopleExpressionFactory = _hostContainer.Resolve<SocialExpressionFactory<Person>>();
                var activityExpressionFactory = _hostContainer.Resolve<SocialExpressionFactory<Activity>>();
                var activityStreamExpressionFactory = _hostContainer.Resolve<SocialExpressionFactory<ActivityEntry>>();
                var messageExpressionFactory = _hostContainer.Resolve<SocialExpressionFactory<Message>>();
                var groupExpressionFactory = _hostContainer.Resolve<SocialExpressionFactory<Group>>();

                var handlers = new List<SocialHandler>
                    {
                        new PeopleHandler<Person>("people", peopleRepository, peopleExpressionFactory), 
                        new ActivityHandler<Activity>("activities", activityRepository, activityExpressionFactory), 
                        new ActivityHandler<ActivityEntry>("activitystreams", activityStreamRepository, activityStreamExpressionFactory), 
                        new MessageHandler<Message>("messages", messagesRepository, messageExpressionFactory),
                        new GroupHandler<Group>("groups", groupsRepository, groupExpressionFactory)
                    };

                _hostContainer.RegisterInstance(typeof(IEnumerable<SocialHandler>), handlers);
            }
        }

        #endregion
    }
}
