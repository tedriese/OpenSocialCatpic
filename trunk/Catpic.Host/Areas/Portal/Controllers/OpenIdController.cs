using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Catpic.Host.Engine.Security;
using Catpic.Host.Engine.Social;
using Catpic.Social.People;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace Catpic.Host.Areas.Portal.Controllers
{
    using Catpic.Social;
    using Catpic.Social.Activities;
    using Catpic.Social.Messages;

    public class OpenIdController : Controller
    {
        private readonly static OpenIdRelyingParty OpenIdProvider = new OpenIdRelyingParty();

        private readonly IDictionary<string, IOpenIdRequestor> _requestors = new Dictionary<string, IOpenIdRequestor>()
            {
                {"https://www.google.com/accounts/o8/id", new GoogleOpenIdRequestor() },
                {"http://myopenid.com/", new MyOpenIdRequestor() }
            };

        private readonly CanonicalDbLoader _loader;

        public OpenIdController(CanonicalDbLoader loader)
        {
            _loader = loader;
        }

        public ActionResult Authenticate(string userOpenId)
        {
            var requestor = _requestors[userOpenId];
            // Provider's response
            IAuthenticationResponse response = OpenIdProvider.GetResponse();

            // no request to provider
            if (response == null)
            {
                  Identifier id;
                // get client's OpenID.
                  if (Identifier.TryParse(userOpenId, out id))
                  {
                      try
                      {
                          IAuthenticationRequest request = OpenIdProvider.CreateRequest(userOpenId);
                          return requestor.RedirectingResponse(request).AsActionResult();
                      }
                      catch (ProtocolException ex)
                      {
                          //Trace.Error(TraceCategory, ex);
                      }
                  }
                return RedirectToAction("Login", "Account");
            }

            // provider's response
            switch (response.Status)
            {
                case AuthenticationStatus.Authenticated:

                    var id = requestor.CallbackResponse(response);
                   //NOTE if there is no user with the same id, add it to repository
                    AddPerson(id);
                    return RedirectToAction("Index", "Home");
                case AuthenticationStatus.Failed:

                    TempData["error"] = response.Exception.Message;
                    return RedirectToAction("Index", "Home");
                case AuthenticationStatus.Canceled:
                default:

                    return RedirectToAction("Index", "Home");
            }
        }

        private void AddPerson(string id)
        {
            // No such user
            if(this._loader.PeopleCollections.All(c => c.UserId != id))
            {
                Person person = new Person();
                person.Id = id;
                person.Name = new Name() { GivenName = id, FamilyName = id };
                person.Thumbnail = @"/Content/Social/Avatars/unknown.jpeg";

                // NOTE register user in collections
                {
                    var selfCollection = new EntityCollection<Person>();
                    selfCollection.Type = "@self";
                    selfCollection.UserId = person.Id;
                    selfCollection.Entities = new List<Person>() { person };

                    var friendsCollection = new EntityCollection<Person>();
                    friendsCollection.Type = "@friends";
                    friendsCollection.UserId = person.Id;
                    friendsCollection.Entities = new List<Person>();

                    _loader.PeopleCollections.Add(selfCollection);
                    _loader.PeopleCollections.Add(friendsCollection);
                }
                {
                    var selfCollection = new EntityCollection<Activity>();
                    selfCollection.Type = "@self";
                    selfCollection.UserId = person.Id;
                    selfCollection.Entities = null;

                    var friendsCollection = new EntityCollection<Activity>();
                    friendsCollection.Type = "@friends";
                    friendsCollection.UserId = person.Id;
                    friendsCollection.Entities = null;

                    _loader.ActivityCollections.Add(selfCollection);
                    _loader.ActivityCollections.Add(friendsCollection);
                }

                {
                    var selfCollection = new EntityCollection<ActivityEntry>();
                    selfCollection.Type = "@self";
                    selfCollection.UserId = person.Id;
                    selfCollection.Entities = null;

                    var friendsCollection = new EntityCollection<ActivityEntry>();
                    friendsCollection.Type = "@friends";
                    friendsCollection.UserId = person.Id;

                    friendsCollection.Entities = null;

                    _loader.ActivityStreamCollections.Add(selfCollection);
                    _loader.ActivityStreamCollections.Add(friendsCollection);
                }

                {
                    var collection = new EntityCollection<Message>();
                    collection.Type = "notification";
                    collection.UserId = person.Id;
                    collection.Title = "Notifications";
                    Message message = new Message()
                        {
                            Id = "1",
                            Title = String.Format("Welcome, {0}", person.Name.GivenName),
                            Body = "We are happy to see you here",
                        };
                    collection.Entities = new Message[1] { message };
                    _loader.MessageCollections.Add(collection);
                }
            }

        }
    }
}