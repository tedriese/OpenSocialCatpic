// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EFMessageRepositoryTests.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;

    using Catpic.Data.EntityFramework;
    using Catpic.Data.EntityFramework.Helpers;
    using Catpic.Data.EntityFramework.Repositories;
    using Catpic.Social;
    using Catpic.Social.People;

    using NUnit.Framework;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    //[TestFixture]
    public class EFMessageRepositoryTests
    {
        //[Test]
        public void CanSendMessage()
        {
            Database.SetInitializer<CatpicContext>(new DropCreateDatabaseAlways<CatpicContext>());

            var peopleRepository = this.GetPeopleRepository();
            var messageRepository = new MessageRepository(null);

            var canonical = CatpicContext.Current(null).Persons.Single(p => p.DisplayName == "Canonical");
            var john = CatpicContext.Current(null).Persons.Single(p => p.DisplayName == "Johnny Doe");

            const string Body = "Hi, John!";
            const string Title = "Test";
            var message = messageRepository.AddEntityAsync(canonical.Id, SocialConsts.PrivateMessageType, new EntityMessage()
                {
                    SenderId = canonical.Id,
                    Recipients = new string[] { john.Id },
                    Body = Body,
                    Title = Title
                }).Result;

            var canonicalMessage = (from c in CatpicContext.Current(null).MessageCollections
                                    where c.UserId == canonical.Id && c.Type == SocialConsts.PrivateMessageType
                                   select c).Single().Entities.Single(m => m.Id == message.Id);

            var johnMessage = (from c in CatpicContext.Current(null).MessageCollections
                                   where c.UserId == canonical.Id && c.Type == SocialConsts.PrivateMessageType
                                   select c).Single().Entities.Single(m => m.Id == message.Id);

            Assert.AreEqual(Body, canonicalMessage.Body);
            Assert.AreEqual(Body, johnMessage.Body);
            Assert.AreEqual(Title, canonicalMessage.Title);
            Assert.AreEqual(Title, johnMessage.Title);
            Assert.AreEqual(canonical.Id, canonicalMessage.SenderId);
            Assert.AreEqual(canonical.Id, johnMessage.SenderId);
        }

        private PersonRepository GetPeopleRepository()
        {
            // add two persons
            SocialFactory factory = new SocialFactory(null);
            var canonical = factory.CreateProfile(new EntityPerson()
            {
                Id = Guid.NewGuid().ToString(),
                Name = new Name()
                {
                    FamilyName = "Canonical",
                    GivenName = "Canonical",
                    Formatted = "Canonical"
                },
                DisplayName = "Canonical"
            });

            var john = factory.CreateProfile(new EntityPerson()
            {
                Id = Guid.NewGuid().ToString(),
                Name = new Name()
                {
                    FamilyName = "Doe",
                    GivenName = "John",
                    Formatted = "John Doe"
                },
                DisplayName = "Johnny Doe"
            });

            // make them friends
            PersonRepository repository = new PersonRepository(null);
            repository.AddEntityAsync(canonical.Id, "@friends", john);

            return repository;
        }
    }
}
