// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestHelper.cs" company="Catpic Software">
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
    using System.Linq;
    using System.Text;

    using Catpic.Data.EntityFramework;
    using Catpic.Data.EntityFramework.Helpers;
    using Catpic.Data.EntityFramework.Repositories;
    using Catpic.Social;
    using Catpic.Social.People;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TestHelper
    {
        public static PersonRepository GetPersonRepository()
        {
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

            var jane = factory.CreateProfile(new EntityPerson()
            {
                Id = Guid.NewGuid().ToString(),
                Name = new Name()
                {
                    FamilyName = "Doe",
                    GivenName = "Jane",
                    Formatted = "Jane Doe"
                },
                DisplayName = "Janny Doe"
            });


            var george = factory.CreateProfile(new EntityPerson()
            {
                Id = Guid.NewGuid().ToString(),
                Name = new Name()
                {
                    FamilyName = "Doe",
                    GivenName = "George",
                    Formatted = "George Doe"
                },
                DisplayName = "George Doe"
            });

            var repository = new PersonRepository(null);
            repository.AddEntityAsync(canonical.Id, "@friends", john);
            repository.AddEntityAsync(canonical.Id, "@friends", jane);
            repository.AddEntityAsync(canonical.Id, "@friends", george);

            return repository;
        }
    }
}
