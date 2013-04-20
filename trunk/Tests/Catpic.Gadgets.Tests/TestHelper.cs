using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using Catpic.Gadgets.Containers;
using Catpic.Gadgets.Format;
using Catpic.Gadgets.Proxies;
using Catpic.Gadgets.Rendering.Container;
using Catpic.Gadgets.Rendering.Gadget;
using Catpic.Gadgets.Security;
using Catpic.Gadgets.Security.OAuth;
using Catpic.Gadgets.Security.OAuth2;
using Catpic.Utils;
using Catpic.Utils.Caching;
using Catpic.Utils.Configuration;
using Rhino.Mocks;

namespace Catpic.Gadgets.Tests
{
    using System.Threading.Tasks;

    using Catpic.Utils.OAuth;

    internal static class TestHelper
    {
        public static IRequestHandler GetRequestHandler()
        {
            var sets = new List<IFeatureSet>();
            foreach (var featureSetConfig in ConfigSettings.Instance.GetSections("features/set"))
            {
                var closure = featureSetConfig;
                sets.Add(ObjectCreator.CreateAndConfigure<IFeatureSet>(closure, "@type", closure));
            }

            var featureProvider = new FeatureProvider(sets);
            var parser = new GadgetParser();
            var cache = new RuntimeMemoryCache();
            var factory = new GadgetDefinitionFactory(parser, (s) => cache);

            //register containers
            var containers = new List<IContainer>();
            foreach (var containerConfig in ConfigSettings.Instance.GetSections("containers/container"))
            {
                var closure = containerConfig;
                containers.Add(ObjectCreator.CreateAndConfigure<IContainer>(closure, "@type", featureProvider, factory, closure));
            }

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

            OAuthConsumerProvider provider = new OAuthConsumerProvider(consumers, null);//TODO

           /* var gadgetRenderModules = new List<GadgetRenderModule>();
            foreach (var renderModuleConfig in ConfigSettings.Instance.GetSections("rendering/gadget/pipeline/module"))
            {
                var renderModule = ObjectCreator.CreateAndConfigure<GadgetRenderModule>(renderModuleConfig);
                gadgetRenderModules.Add(renderModule);
            }*/

            var gadgetRenderModules = new List<IGadgetRenderModule>
                    {
                        new ViewGadgetRenderModule(),
                        new FeatureGadgetRenderModule(),
                        new UserPreferencesGadgetRenderModule(),
                        new MessageGadgetRenderModule(),
                        new UtilGadgetRenderModule(cache),
                        new ConcatGadgetRenderModule()
                    };
            /*var containerRenderModules = new List<IContainerRenderModule>();
            foreach (var renderModuleConfig in ConfigSettings.Instance.GetSections("rendering/container/pipeline/module"))
            {
                var renderModule = ObjectCreator.CreateAndConfigure<IContainerRenderModule>(renderModuleConfig);
                containerRenderModules.Add(renderModule);
            }*/

            var containerRenderModules = new List<IContainerRenderModule>
                    {
                        new FeatureContainerRenderModule(), 
                        new MetadataContainerRenderModule(cache) 
                    };

            OAuthRequestHandler oauthRequestHandler = new OAuthRequestHandler(factory, provider, 
                "http://localhost:9999/gadgets/oauth_callback", (s) => cache);

            OAuth2RequestHandler oauth2RequestHandler = new OAuth2RequestHandler(factory, provider,
              "http://localhost:9999/gadgets/oauth_callback", (s) => cache);

            SecurityRequestHandler securityRequestHandler = new SecurityRequestHandler(
                new List<IOAuthRequestHandler> {oauthRequestHandler, oauth2RequestHandler});

            return new RequestHandler(new ContainerProvider(containers),
                new ContainerRenderPipeline(containerRenderModules), new GadgetRenderPipeline(gadgetRenderModules),
                factory, securityRequestHandler, new RequestProxy(), new ConcatProxy());
        }

        public static HttpContextBase GetContext(HttpRequestBase request, HttpResponseBase response)
        {
            var context = MockRepository.GenerateMock<HttpContextBase>();
            context.Stub(c => c.Request).Return(request);
            context.Stub(c => c.Response).Return(response);
            context.Stub(c => c.User).Return(new GenericPrincipal(new GenericIdentity("john.doe"), new string[] {}));
            return context;
        }

        public static HttpRequestBase GetRequest(string url, string renderType = "iframe", string view = "default")
        {
            //set up mock
            var request = MockRepository.GenerateMock<HttpRequestBase>();
            NameValueCollection sq = new NameValueCollection()
                                         {
                                             {"container", "default"},
                                             {"mid","0"},
                                             {"nocache","1"},
                                             {"country","ALL"},
                                             {"lang","ALL"},
                                             {"renderType", renderType},
                                             {"view",view},
                                             {"parent","http://localhost:9999"},
                                             {"st","john.doe:john.doe:appid:cont:url:0:default"},
                                             {"url",url},
                                             {"rpctoken","1809198711"}
                                         };
            request.Stub(r => r.QueryString).Return(sq);
            request.Stub(r => r.Params).Return(sq);
            request.Stub(r => r.HttpMethod).Return("GET");
            return request;
        }

        public static HttpResponseBase GetResponse(TextWriter writer)
        {
            // set up mock
            var response = MockRepository.GenerateMock<HttpResponseBase>();
            response.Stub(r => r.Output).Return(writer);
            return response;
        }

        public static Task GetCreateGadgetTask(string gadget, string renderType, StringWriter writer, string view = "default")
        {
            var request = TestHelper.GetRequest(gadget, renderType, view);
            var response = TestHelper.GetResponse(writer);
            var context = TestHelper.GetContext(request, response);
            var requestHandler = TestHelper.GetRequestHandler();
            ICache cache = new RuntimeMemoryCache();
            ICryptoService cryptoService = new AESCryptoService("mysecret");
            SecurityTokenFactory tokenFactory = new SecurityTokenFactory(cryptoService, c => cache);
            var token = tokenFactory.Create(context);
            return requestHandler.CreateGadgetAsync(new GadgetContext(context, token));
        }
    }
}
