using System;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Catpic.Gadgets.Security;
using Catpic.Host.Engine;
using Catpic.Host.Engine.Security;
using Catpic.Host.Engine.Unity;
using Catpic.Utils;
using Catpic.Web.Configuration;
using Microsoft.Practices.Unity;

namespace Catpic.Host
{
    using Catpic.Web.Security;

    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly object _lock = new object();
        private static volatile IUnityContainer _container;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfigurator.RegisterGlobalFilters(GlobalFilters.Filters);

            var unityResolver = new UnityDependencyResolver(Container);

            DependencyResolver.SetResolver(unityResolver); //mvc 3
            GlobalConfiguration.Configuration.DependencyResolver = unityResolver; //webapi rc
            CatpicConfigurator.ConfigureRoutes(GlobalConfiguration.Configuration, RouteTable.Routes);
            CatpicConfigurator.ConfigureFormatters(GlobalConfiguration.Configuration);
            RouteConfigurator.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            IIdentity identity;
            ISecurityToken token;
            if (authCookie != null)
            {
                token = TokenFactory.Create(new HttpContextWrapper(Context));
                //Extract the forms authentication cookie
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                identity = new GenericIdentity(authTicket.Name);
            }
            else
            {
                //build anonymous identity
                token = TokenFactory.CreateAnonymous(new HttpContextWrapper(Context));
                identity = new GenericIdentity(Consts.AnonymousId); 
            }

            IPrincipal user = new CatpicPrincipal(token, identity, new string[] { });
            Context.User = user;

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ctx = HttpContext.Current;
            var ex = ctx.Server.GetLastError();

            if (ex != null)
                TraceFactory.GetTrace().Error("host", "unhandled exception:", ex);
        }

        #region Public properties

        private static string _buildVersion;
        public static string BuildVersion
        {
            get
            {
                if (_buildVersion == null)
                {
                    string path = HttpContext.Current.Request.PhysicalApplicationPath + @"\bin\Catpic.Gadgets.dll";
                    if (path != string.Empty)
                    {
                        Assembly asm = Assembly.ReflectionOnlyLoadFrom(path);
                        _buildVersion = asm.GetName().Version.ToString();
                    }
                    else
                    {
                        _buildVersion = string.Empty;
                    }
                }
                return _buildVersion;
            }
        }

        private static DateTime _buildDate = DateTime.MinValue;
        public static DateTime BuildDate
        {
            get
            {
                if (_buildDate == DateTime.MinValue)
                    _buildDate = new System.IO.FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;

                return _buildDate;
            }
        }

        #endregion

        #region Private properties

        private volatile ISecurityTokenFactory _tokenFactory;
        private ISecurityTokenFactory TokenFactory
        {
            get
            {
                if (_tokenFactory == null)
                {
                    lock (_lock)
                    {
                        if (_tokenFactory == null)
                            _tokenFactory = Container.Resolve<ISecurityTokenFactory>();
                    }
                }
                return _tokenFactory;
            }
        }

        private static IUnityContainer Container
        {
            get
            {
                if(_container == null)
                {
                    lock (_lock)
                    {
                        if (_container == null)
                        {
                            var container = new UnityContainer();
                            HostConfigurator.Configure(container);
                            _container = container;
                        }
                    }
                }
                return _container;
            }
        }

        #endregion
    }
}