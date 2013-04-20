// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EFGroupRepositoryTests.cs" company="Catpic Software">
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
    using Catpic.Data.EntityFramework.Repositories;
    using Catpic.Social;

    using NUnit.Framework;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    //[TestFixture]
    public class EFGroupRepositoryTests
    {
        //[Test]
        public void CanAddAndSelect()
        {
            // force to recreate db
           /* Database.SetInitializer<CatpicContext>(new DropCreateDatabaseAlways<CatpicContext>());
            var personRepository = TestHelper.GetPersonRepository();

            var canonical = CatpicContext.Current(null).Persons.Single(p => p.DisplayName == "Canonical");
            var john = CatpicContext.Current(null).Persons.Single(p => p.DisplayName == "Johnny Doe");

            GroupRepository repository = new GroupRepository();



            repository.AddEntityAsync(canonical.Id, SocialConsts.GroupIdSelf, new EntityGroup()
                {
                    
                });*/
        }


    }
}
