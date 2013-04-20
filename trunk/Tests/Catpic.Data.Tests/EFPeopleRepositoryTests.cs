using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catpic.Data.Tests
{
    using System.Data.Entity;

    using Catpic.Data.EntityFramework;
    using Catpic.Data.EntityFramework.Repositories;
    using Catpic.Social;
    using Catpic.Social.Formatting;
    using Catpic.Social.People;

    using NUnit.Framework;

    //[TestFixture]
    public class EFPeopleRepositoryTests
    {
        // NOTE Removed as environment specific (need to set appropriate connection string for CI build at apphb.com)
        //[Test]
        public void CanAddAndSelect()
        {
            Database.SetInitializer<CatpicContext>(new DropCreateDatabaseAlways<CatpicContext>());

            var repository = TestHelper.GetPersonRepository();

            var expressionFactory = new SocialExpressionFactory<EntityPerson>();
            var canonical = CatpicContext.Current(null).Persons.Single(p => p.DisplayName == "Canonical");
            var expression = expressionFactory.CreateEntityListExpression(
                canonical.Id,
                SocialConsts.GroupIdFriends,
                new CollectionItem()
                    {
                        FilterBy = "displayName",
                        FilterOp = "contains",
                        FilterValue = "Doe",
                        SortBy = "displayName",
                        StartIndex = 0,
                        Count = 2,
                        Fields = new string[2] { "id", "displayName" }
                    },
                repository.GetQueryable().Expression);

            var task2 = repository.Select(expression);
            task2.Wait();
            var result = (task2.Result as IEnumerable<dynamic>).ToList();
            Assert.AreEqual("George Doe", result[0].displayName);
            Assert.AreEqual("Janny Doe", result[1].displayName);
        }

        //[Test]
        public void CanUpdateAndSelect()
        {
            Database.SetInitializer<CatpicContext>(new DropCreateDatabaseAlways<CatpicContext>());

            var repository = TestHelper.GetPersonRepository();

            var canonical = CatpicContext.Current(null).Persons.Single(p => p.DisplayName == "Canonical");
            canonical.DisplayName = "Changed";
            repository.UpdateEntityAsync(canonical.Id, "@self", canonical);

            var changed = CatpicContext.Current(null).Persons.Single(p => p.DisplayName == "Changed");

            Assert.AreEqual("Changed", changed.DisplayName);
        }

    }
}
