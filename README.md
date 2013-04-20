OpenSocialNet
=============

Catpic is an open source .NET project which implements OpenSocial container specification and provides the ability to host OpenSocial gadgets - simple HTML and JavaScript applications that can be embedded in webpages and other apps. Gadgets are developed using the OpenSocial Gadgets API and basic web technologies such as XML, JavaScript, Flash. In general, Catpic gadget server consists of the following parts:

OpenSocial Container: implementation of the public specification that defines a component hosting environment (container) and a set of common application programming interfaces (APIs) for social networking web-based applications
Gadget Container JavaScript: core JavaScript foundation for general gadget functionality. This JavaScript manages security, communication, UI layout, and feature extensions. Apache Shindig implementation is used by default.
Gadget Rendering Server: used to render the gadget XML into JavaScript and HTML for the container to expose via the container JavaScript. This part is written on .NET and common modules can be replaced/extended by custom ones.
Catpic provides:

OpenSocial container: people, activities, activity streams, messages
implementation of google gadgets specification
ability to host platform-independed components - gadgets
fully .NET implementation on server side
extensibility by custom modules
simple integration into existing ASP.NET/ ASP.NET MVC application via NuGet package (in development)

try the latest build here: http://catpic.apphb.com/

twitter: https://twitter.com/catpic_software

facebook: http://www.facebook.com/CatpicTestCommunity

original repository: http://catpic.codeplex.com/