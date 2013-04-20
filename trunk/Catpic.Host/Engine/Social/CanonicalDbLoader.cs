// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CanonicalDbLoader.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Loads shindig's test database
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Host.Engine.Social
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Catpic.Social;
    using Catpic.Social.Activities;
    using Catpic.Social.Groups;
    using Catpic.Social.Messages;
    using Catpic.Social.People;
    using Catpic.Utils;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Message = Catpic.Social.Messages.Message;

    /// <summary>
    /// Loads shindig's test database
    /// </summary>
    public class CanonicalDbLoader
    {
        private readonly JObject _database;

        public CanonicalDbLoader(string databasePath)
        {
            var content = FileHelper.GetContent(databasePath);
            var normalizedJson = JsonHelper.Uncomment(content);
            _database = JsonConvert.DeserializeObject<JObject>(normalizedJson);
            var get = PeopleCollections.Select(a =>a );
        }

        #region People

        private IList<EntityCollection<Person>> _peopleCollections;
        public IList<EntityCollection<Person>> PeopleCollections
        {
            get
            {
                if (_peopleCollections == null)
                {
                    List<Person> people = new List<Person>();

                    //create people list
                    foreach(var jPerson in _database["people"])
                    {
                        var p = new Person
                        {
                            Id = jPerson["id"].Value<string>(),
                            DisplayName = jPerson["displayName"].Value<string>(),
                            Gender = jPerson["gender"].Value<string>(),
                            Thumbnail = jPerson["thumbnailUrl"].Value<string>(),
                            Name = new Name()
                            {
                                FamilyName = jPerson["name"]["familyName"].Value<string>(),
                                GivenName = jPerson["name"]["givenName"].Value<string>(),
                                Formatted = jPerson["name"]["formatted"].Value<string>()
                            }
                        };

                        people.Add(p);
                    }

                    // create friend relationships
                    // SetFriendRelationships(people);
                    CreatePeopleCollection(people);
                }
                return _peopleCollections;
            }
        }

        private void SetFriendRelationships(IList<Person> people)
        {
            if(people == null)
                throw new InvalidOperationException("Unable to create friend relationships: people list is null");
            var friendLinks = _database["friendLinks"];

            foreach (JProperty friendLink in friendLinks)
            {
                List<Person> friends = new List<Person>();
                foreach (var jFriend in friendLink.Value)
                {
                    var friendId = jFriend.Value<string>();
                    var friend = people.Single(p => p.Id == friendId);
                    friends.Add(friend);
                }
            }
        }

        private void CreatePeopleCollection(IList<Person> people)
        {
            _peopleCollections = new List<EntityCollection<Person>>();
            foreach (var person  in people)
            {
                var selfCollection = new EntityCollection<Person>();
                selfCollection.Type = "@self";
                selfCollection.UserId = person.Id;
                selfCollection.Entities = new List<Person>() { person };

                var friendsCollection = new EntityCollection<Person>();
                friendsCollection.Type = "@friends";
                friendsCollection.UserId = person.Id;
                friendsCollection.Entities = GetFriends(person, people);

                _peopleCollections.Add(selfCollection);
                _peopleCollections.Add(friendsCollection);
            }
        }

        private IList<Person> GetFriends(Person person, IList<Person> people)
        {
            var friendLinks = _database["friendLinks"];

            foreach (JProperty friendLink in friendLinks)
            {
                if (friendLink.Name == person.Id)
                {
                    List<Person> friends = new List<Person>();
                    foreach (var jFriend in friendLink.Value)
                    {
                        var friendId = jFriend.Value<string>();
                        var friend = people.Single(p => p.Id == friendId);
                        friends.Add(friend);
                    }
                    return friends;
                }
            }
            return new List<Person>();
        }

        #endregion

        #region Activity

        private IList<EntityCollection<Activity>> _activityCollections;
        public IList<EntityCollection<Activity>> ActivityCollections
        {
            get
            {
                if (_activityCollections == null)
                {
                    List<Activity> activities = new List<Activity>();
                    foreach (JProperty jPerson in _database["activities"])
                    {
                        foreach (var jActivity in jPerson.Value)
                        {
                            Activity activity = new Activity();
                            activity.UserId = jPerson.Name;
                            activity.Body = jActivity["body"].Value<string>();
                            activity.Id = jActivity["id"].Value<string>();
                            if (jActivity["streamTitle"] != null)
                                activity.StreamTitle = jActivity["streamTitle"].Value<string>();
                            activity.Title = jActivity["title"].Value<string>();
                            var jMediaItems = jActivity["mediaItems"];
                            if (jMediaItems != null)
                            {
                                List<MediaItem> mediaItems = new List<MediaItem>();
                                foreach (var jMediaItem in jMediaItems)
                                {
                                    mediaItems.Add(new MediaItem()
                                    {
                                        MimeType = jMediaItem["mimeType"].Value<string>(),
                                        Type = jMediaItem["type"].Value<string>(),
                                        Url = jMediaItem["url"].Value<string>(),
                                    });
                                }
                                activity.MediaItems = mediaItems;
                            }
                            activities.Add(activity);
                        }
                    }
                    CreateActivityCollections(activities);
                   
                }
                return _activityCollections;
            }
        }

        private void CreateActivityCollections(IList<Activity> activities)
        {
            _activityCollections = new List<EntityCollection<Activity>>();
            var people = _peopleCollections.SelectMany(c => c.Entities).Distinct();
            foreach (var person in people)
            {
                var closure = person;
                var userActivities = activities.Where(a => a.UserId == closure.Id);
                var selfCollection = new EntityCollection<Activity>();
                selfCollection.Type = "@self";
                selfCollection.UserId = person.Id;
                selfCollection.Entities = userActivities.ToList();

                var friendsCollection = new EntityCollection<Activity>();
                friendsCollection.Type = "@friends";
                friendsCollection.UserId = person.Id;

                var friendIds = GetFriends(person).Select(p => p.Id);
                friendsCollection.Entities = activities.Where(a => friendIds.Contains(a.UserId)).ToList();

                _activityCollections.Add(selfCollection);
                _activityCollections.Add(friendsCollection);
            }
        }

        #endregion

        #region ActivityStreams

        private IList<EntityCollection<ActivityEntry>> _activityStreamCollections;
        public IList<EntityCollection<ActivityEntry>> ActivityStreamCollections
        {
            get
            {
                if (_activityStreamCollections == null)
                {
                     List<ActivityEntry> activityStreams = new List<ActivityEntry>();
                     foreach (JProperty jPerson in _database["activityEntries"])
                     {
                         foreach (var jActivityStream in jPerson.Value)
                         {
                            ActivityEntry activityEntry = new ActivityEntry();
                            // for internal purposes
                            activityEntry.UserId = jPerson.Name;
                            //
                            activityEntry.Id = jActivityStream["id"].Value<string>();
                             activityEntry.Published = jActivityStream["published"].Value<DateTime>();
                             activityEntry.Title = jActivityStream["title"].Value<string>();
                             activityEntry.Verb = jActivityStream["verb"].Value<string>();
                             var jActor = jActivityStream["actor"];
                            var image = jActor["image"];
                            activityEntry.Actor = new ActivityObject()
                            {
                                Id = jPerson.Name,
                                DisplayName = jActor["displayName"].Value<string>(),
                                Image = new MediaLink()
                                {
                                    Url = image["url"].Value<string>(),
                                    Height = image["height"].Value<int>(),
                                    Width = image["width"].Value<int>()
                                },
                                ObjectType = jActor["objectType"].Value<string>(),
                                Url = jActor["url"].Value<string>(),
                            };
                            var @object = jActivityStream["object"];
                             activityEntry.ObjectEntry = new ActivityObject()
                                {
                                    Id = @object["id"].Value<string>(),
                                    Url = @object["url"].Value<string>(),
                                };
                             var target = jActivityStream["target"];
                             activityEntry.TargetEntry = new ActivityObject()
                                {
                                    Id = target["id"].Value<string>(),
                                    ObjectType = target["objectType"].Value<string>(),
                                    Url = target["url"].Value<string>(),
                                    DisplayName = target["displayName"].Value<string>()
                                };
                             activityStreams.Add(activityEntry);
                         }
                     }
                     this.CreateActivityStreamsCollections(activityStreams);
                }
                return _activityStreamCollections;
            }
        }

        private void CreateActivityStreamsCollections(IList<ActivityEntry> activities)
        {
            _activityStreamCollections = new List<EntityCollection<ActivityEntry>>();
            var people = _peopleCollections.SelectMany(c => c.Entities).Distinct();
            foreach (var person in people)
            {
                var closure = person;
                var userActivities = activities.Where(a => a.UserId == closure.Id);
                var selfCollection = new EntityCollection<ActivityEntry>();
                selfCollection.Type = "@self";
                selfCollection.UserId = person.Id;
                selfCollection.Entities = userActivities.ToList();

                var friendsCollection = new EntityCollection<ActivityEntry>();
                friendsCollection.Type = "@friends";
                friendsCollection.UserId = person.Id;

                var friendIds = GetFriends(person).Select(p => p.Id);
                friendsCollection.Entities = activities.Where(a => friendIds.Contains(a.UserId)).ToList();

                _activityStreamCollections.Add(selfCollection);
                _activityStreamCollections.Add(friendsCollection);
            }
        }

        #endregion

        #region Messages

        private IList<EntityCollection<Message>> _messageCollections;
        public IList<EntityCollection<Message>> MessageCollections
        {
            get
            {
                 if (this._messageCollections == null)
                 {
                     var collections = new List<EntityCollection<Message>>();
                     foreach (JProperty jPerson in this._database["messages"])
                     {
                         foreach (JProperty jCollection in jPerson.Values())
                         {
                             var collection = new EntityCollection<Message>();
                             collection.Type = jCollection.Name;
                             collection.UserId = jPerson.Name;
                             collection.Title = jCollection.Value["title"].Value<string>();
                             var messages = new List<Message>();
                             foreach (var jMessage in jCollection.Value["messages"])
                             {
                                 // TODO init all fields
                                 var message = new Message();
                                 message.Id = JsonHelper.SafeGetStringParam("id", jMessage);
                                 message.Title = JsonHelper.SafeGetStringParam("title", jMessage);
                                 message.Type = JsonHelper.SafeGetStringParam("type", jMessage);
                                 message.Body = JsonHelper.SafeGetStringParam("body", jMessage);
                                 message.SenderId = JsonHelper.SafeGetStringParam("senderId", jMessage);
                                 message.InReplyTo = JsonHelper.SafeGetStringParam("inReplyTo", jMessage);
                                 message.Replies = JsonHelper.SafeGetArrayParams("replies", jMessage);
                                 messages.Add(message);
                             }

                             collection.Entities = messages;
                             collections.Add(collection);
                         }
                     }
                     
                     this._messageCollections = collections;
                 }

                return this._messageCollections;
            }
        }

        #endregion

        #region Groups

        private IList<EntityCollection<Group>> _groupCollections;
        public IList<EntityCollection<Group>> GroupCollections
        {
            get
            {
                if (_groupCollections == null)
                {
                    List<Group> groups = new List<Group>();
                    
                    // create groups
                    foreach (JProperty jMember  in _database["groups"])
                    {
                        foreach (var jGroup in jMember.Value)
                        {
                            Group group = new Group();
                            group.Id = jGroup["id"]["value"].Value<string>();
                            group.Title = jGroup["title"].Value<string>();
                            group.Description = jGroup["description"].Value<string>();

                            if(groups.All(g => g.Id != group.Id))
                            {
                                groups.Add(group);
                            }
                        }
                    }

                    IList<EntityCollection<Group>> groupCollections = new List<EntityCollection<Group>>();
                    foreach (JProperty jGroup in _database["groupMembers"])
                    {
                        foreach (var jMember in jGroup.Value)
                        {
                            string groupId = jGroup.Name;
                            string userId = jMember.Value<string>();
                            var groupCollection = groupCollections.SingleOrDefault(g => g.UserId == userId);
                            if (groupCollection == null)
                            {
                                groupCollection = new EntityCollection<Group> { Type = "@self", UserId = userId, Entities = new List<Group>() };
                                groupCollections.Add(groupCollection);
                            }

                            var group = groups.Single(g => g.Id == groupId);
                            ((List<Group>)groupCollection.Entities).Add(group);
                        }
                    }

                    _groupCollections = groupCollections;
                }

                return _groupCollections;
            }
        }

        #endregion

        private IList<Person> GetFriends(Person person)
        {
            return PeopleCollections.Single(p => p.UserId == person.Id && p.Type == "@friends").Entities as IList<Person>;
        }
    }
}