// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SocialFactory.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Creates user profile using EF
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Data.EntityFramework
{
    using System;
    using System.Collections.Generic;

    using Catpic.Data.EntityFramework.Repositories;
    using Catpic.Social;
    using Catpic.Social.People;

    /// <summary>
    /// Creates user profile using EF
    /// </summary>
    public class SocialFactory : ISocialFactory<EntityPerson>
    {
        /// <summary>
        /// Default group names
        /// </summary>
        private static readonly IEnumerable<string> GroupCollectionNames = new string[]
            {
                SocialConsts.GroupIdSelf,
                SocialConsts.GroupIdFriends
            };

        /// <summary>
        /// Default message collections
        /// </summary>
        private static readonly IEnumerable<string> MessageCollectionNames = new string[]
            {
              SocialConsts.NotificationMessageType,
              SocialConsts.PrivateMessageType,
             // SocialConsts.PublicMessageType,
            };

        /// <summary>
        /// Connection string
        /// </summary>
        private readonly string _connectionString;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SocialFactory"/> class.
        /// </summary>
        /// <param name="connStr"> Connenction string. </param>
        public SocialFactory(string connStr)
        {
            this._connectionString = connStr;
        }

        #region Implementation of ISocialFactory<EntityPerson>

        /// <summary>
        /// Creates person
        /// </summary>
        /// <typeparam name="T"> Person entity </typeparam>
        /// <param name="person"> The person.  </param>
        /// <returns> Created person </returns>
        public EntityPerson CreateProfile(EntityPerson person)
        {
            return CreatePerson(person, this._connectionString);
        }

        #endregion

        /// <summary>
        /// Creates person data and related things: activities, messages
        /// </summary>
        /// <param name="entity"> Person entity. </param>
        /// <param name="connectionString"> Connection string. </param>
        /// <returns> Created person </returns>
        private static EntityPerson CreatePerson(EntityPerson entity, string connectionString)
        {
            if (string.IsNullOrEmpty(entity.Id))
            {
                entity.Id = Guid.NewGuid().ToString();
            }

            CatpicContext.Current(connectionString).Persons.Add(entity);

            // add self
            AddCollectionWithEntities(
                new EntityCollection<EntityPerson>
                {
                    Type = SocialConsts.GroupIdSelf,
                    Title = null,
                    UserId = entity.Id
                },
                new List<EntityPerson> { entity },
                connectionString);

            // add friends
            AddCollectionWithEntities(
                new EntityCollection<EntityPerson>
                {
                    Type = SocialConsts.GroupIdFriends,
                    Title = null,
                    UserId = entity.Id
                },
                new List<EntityPerson>(), 
                connectionString);

            CreateActivityStreamCollections(entity, connectionString);
            CreateMessageCollections(entity, connectionString);
            CreateGroupCollections(entity, connectionString);

            CatpicContext.Current(connectionString).SaveChanges();

            return entity;
        }

        /// <summary>
        /// Adds person collection with entities
        /// </summary>
        /// <param name="collection"> The collection. </param>
        /// <param name="persons"> The persons. </param>
        /// <param name="connectionString"> The connection string. </param>
        private static void AddCollectionWithEntities(EntityCollection<EntityPerson> collection, ICollection<EntityPerson> persons, string connectionString)
        {
            var copy = new EntityPersonCollection();
            copy.Id = Guid.NewGuid().ToString();
            copy.Type = collection.Type;
            copy.UserId = collection.UserId;
            copy.Title = collection.Title;
            copy.Entities = persons;
            CatpicContext.Current(connectionString).PersonCollections.Add(copy);
        }

        /// <summary>
        /// Creates predefined activity streams for person
        /// </summary>
        /// <param name="person"> The person. </param>
        /// <param name="connectionString"> The connection string. </param>
        private static void CreateActivityStreamCollections(Person person, string connectionString)
        {
            foreach (var groupName in GroupCollectionNames)
            {
                var activityCollection = new EntityActivityEntryCollection()
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = groupName,
                    UserId = person.Id,
                    Entities = new List<EntityActivityEntry>()
                };
                CatpicContext.Current(connectionString).ActivityEntryCollections.Add(activityCollection);
            }
        }

        /// <summary>
        /// Creates default message collections
        /// </summary>
        /// <param name="person"> The person. </param>
        /// <param name="connectionString"> The connection string. </param>
        private static void CreateMessageCollections(Person person, string connectionString)
        {
            foreach (var messageCollectionName in MessageCollectionNames)
            {
                var messageCollection = new EntityMessageCollection()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = messageCollectionName,
                        UserId = person.Id,
                        Entities = new List<EntityMessage>()
                    };
                CatpicContext.Current(connectionString).MessageCollections.Add(messageCollection);
            }
        }

        /// <summary>
        /// Creates Group collection for person
        /// </summary>
        /// <param name="person"> The person. </param>
        /// <param name="connectionString"> The connection string. </param>
        private static void CreateGroupCollections(Person person, string connectionString)
        {
            foreach (var groupName in GroupCollectionNames)
            {
                var groupCollection = new EntityGroupCollection()
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = groupName,
                    UserId = person.Id,
                    Entities = new List<EntityGroup>()
                };
                CatpicContext.Current(connectionString).GroupCollections.Add(groupCollection);
            }
        }
    }
}
