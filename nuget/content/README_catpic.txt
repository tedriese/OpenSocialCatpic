Catpic is an open source .NET project which tries to implement OpenSocial Container specification (see opensocial.org for details). Also it provides ability to host Google gadgets - simple HTML and JavaScript applications that can be embedded in webpages and other apps. Gadgets are developed by Google and third-party developers using the Google Gadgets API and basic web technologies such as XML, JavaScript, Flash. In general, Catpic server consists of the following parts:

	* OpenSocial container implementation
	* Gadget Container JavaScript: core JavaScript foundation for general gadget functionality. This JavaScript manages security, communication, UI layout, and feature extensions. Apache Shindig implementation is used by default.
	* Gadget Rendering Server: used to render the gadget XML into JavaScript and HTML for the container to expose via the container JavaScript. This part is written on .NET and common modules can be replaced/extended by custom ones. 
	
Catpic nuget package includes the following files:
	* binary assemblies: 
		* Catpic.Gadgets.dll - gadget server functionality 
		* Catpic.Social.dll - social functionality
		* Catpic.Web - mvc/web api functionality
		* Catpic.Utils.dll - utilities
		* Catpic.Data.EntityFramework - data layer (experimental)
	* ~/catpic/features: javascript API
	* Sample classes and gadgets in ~/catpic directory
	
IMPORTANT! After sucessful installation you need to register all services in Application_Start event of Global.asax:

	using System.Web.Http;
	...
	protected void Application_Start()
	{
		AreaRegistration.RegisterAllAreas();

		RegisterGlobalFilters(GlobalFilters.Filters);
		RegisterCatpic(); // add this to standard template
		RegisterRoutes(RouteTable.Routes);
	}
		
	private void RegisterCatpic()
	{
		var container = new Microsoft.Practices.Unity.UnityContainer();
		YourApp.HostConfigurator.Configure(container);

		var unityResolver = new YourApp.UnityDependencyResolver(container);

		DependencyResolver.SetResolver(unityResolver); //mvc 3
		System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = unityResolver; //webapi rc

		Catpic.Web.Configuration.CatpicConfigurator.ConfigureRoutes(System.Web.Http.GlobalConfiguration.Configuration, RouteTable.Routes);
        Catpic.Web.Configuration.CatpicConfigurator.ConfigureFormatters(System.Web.Http.GlobalConfiguration.Configuration);
	}

Finally you can check the following demo gadgets:
	* Social
		http://localhost/gadgets/ifr?url=~/catpic/gadgets/Profile.xml	
		http://localhost/gadgets/ifr?url=~/catpic/gadgets/Friends.xml	
		http://localhost/gadgets/ifr?url=~/catpic/gadgets/ActivityStreams.xml
		http://localhost/gadgets/ifr?url=~/catpic/gadgets/Messages.xml
	* Remote gadgets
		http://localhost/gadgets/ifr?url=http://www.google.com/ig/modules/youtube.xml
		http://localhost/gadgets/ifr?url=http://hosting.gmodules.com/ig/gadgets/file/112581010116074801021/hamster.xml

Also you can explore Catpic.Host project from sources at catpic.codeplex.com. It provides the full working example.