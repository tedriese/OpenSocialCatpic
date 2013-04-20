// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EFActvityRepositoryTests.cs" company="Catpic Software">
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
    using Catpic.Social.Activities;
    using Catpic.Social.Formatting;

    using NUnit.Framework;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    //[TestFixture]
    public class EFActivityRepositoryTests
    {
        //[Test]
        public void CanAddAndFriendSelect()
        {
            // force to recreate db
            Database.SetInitializer<CatpicContext>(new DropCreateDatabaseAlways<CatpicContext>());
            var personRepository = TestHelper.GetPersonRepository();

            var activityRepository = new ActivityEntryRepository(null);

            var canonical = CatpicContext.Current(null).Persons.Single(p => p.DisplayName == "Canonical");

            var activity = CreateActivity(canonical.Id);


            // select john doe's friend activities
            var john = CatpicContext.Current(null).Persons.Single(p => p.DisplayName == "Johnny Doe");
            var expressionFactory = new SocialExpressionFactory<EntityActivityEntry>();
            var expression = expressionFactory.CreateEntityListExpression(
                john.Id,
                SocialConsts.GroupIdFriends,
                new CollectionItem()
                {
                    SortBy = "id",
                    StartIndex = 0,
                    Count = 25
                },
                activityRepository.GetQueryable().Expression);
            var task = activityRepository.Select(expression);
            task.Wait();
            var result = (task.Result as IEnumerable<dynamic>).ToList()[0] as EntityActivityEntry;

            Assert.NotNull(result);
            Assert.AreEqual(activity.Title, result.Title);
            Assert.AreEqual(activity.UserId, result.UserId);
            Assert.AreEqual(activity.Published, result.Published);
        }

        //[Test]
        public void CanDelete()
        {
            // force to recreate db
            Database.SetInitializer<CatpicContext>(new DropCreateDatabaseAlways<CatpicContext>());
            var personRepository = TestHelper.GetPersonRepository();

            var activityRepository = new ActivityEntryRepository(null);

            var canonical = CatpicContext.Current(null).Persons.Single(p => p.DisplayName == "Canonical");

            var activity = CreateActivity(canonical.Id);

            // test whether we can delete only by id
            activityRepository.DeleteEntityAsync(canonical.Id, SocialConsts.GroupIdSelf, new EntityActivityEntry() { Id = activity.Id });

            Assert.False(CatpicContext.Current(null).ActivityEntryCollections.Single(
                    a => a.UserId == canonical.Id && a.Type == SocialConsts.GroupIdSelf).Entities.Any(
                        a => a.Id == activity.Id));


        }

        private EntityActivityEntry CreateActivity(string userId)
        {
            var activityRepository = new ActivityEntryRepository(null);
            return activityRepository.AddEntityAsync(
                 userId,
                 SocialConsts.GroupIdSelf,
                 new EntityActivityEntry()
                 {
                     Id = Guid.NewGuid().ToString(),
                     Title = "Test entry",
                     Actor =
                         new ActivityObject()
                         {
                             Id = Guid.NewGuid().ToString(),
                             DisplayName = userId,
                             Image = new MediaLink() { MediaItemId = 0 }
                         },
                     ObjectEntry =
                         new ActivityObject()
                         {
                             Id = Guid.NewGuid().ToString(),
                             DisplayName = userId,
                             Image = new MediaLink() { MediaItemId = 0 }
                         },
                     TargetEntry =
                         new ActivityObject()
                         {
                             Id = Guid.NewGuid().ToString(),
                             DisplayName = userId,
                             Image = new MediaLink() { MediaItemId = 0 }
                         },
                     Published = DateTime.Now,
                     UserId = userId,
                 }).Result as EntityActivityEntry;
        }
    }
}
