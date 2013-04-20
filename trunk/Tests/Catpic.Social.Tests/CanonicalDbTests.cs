using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using Catpic.Host.Engine.Social;

namespace Catpic.Social.Tests
{
    [TestFixture]
    public class CanonicalDbTests
    {
        [Test]
        public void CanLoadPeople()
        {
            //assign
            string path = @"canonicaldb.json";

            //act
            CanonicalDbLoader loader = new CanonicalDbLoader(path);
            var people = loader.PeopleCollections;

            //assert
            Assert.IsNotNull(people);
            var person = people.Single(p => p.Type == "@self" && p.UserId == "john.doe").Entities.Single();
            Assert.IsNotNull(person);
            Assert.AreEqual("Doe", person.Name.FamilyName);
            Assert.AreEqual("John", person.Name.GivenName);
            Assert.AreEqual("John Doe", person.Name.Formatted);
            Assert.AreEqual("male", person.Gender);
            Assert.AreEqual("Johnny", person.DisplayName);
        }

        [Test]
        public void CanLoadActivities()
        {
            //assign
            string path = @"canonicaldb.json";

            //act
            CanonicalDbLoader loader = new CanonicalDbLoader(path);
            var activities = loader.ActivityCollections;

            //assert
            Assert.IsNotNull(activities);
            var activity = activities.SingleOrDefault(p => p.Type == "@self" && p.UserId == "jane.doe").Entities.Single(a => a.Id == "1");
            Assert.IsNotNull(activity);
            Assert.AreEqual("1", activity.Id);
            Assert.AreEqual("jane.doe", activity.UserId);
            Assert.AreEqual("and she thinks you look like him", activity.Body);
            Assert.AreEqual(2, activity.MediaItems.Count());
            Assert.AreEqual("Jane just posted a photo of a monkey", activity.Title);
            Assert.AreEqual("jane's photos", activity.StreamTitle);

            var mediaItem = activity.MediaItems.First();
            Assert.AreEqual(@"image/jpeg", mediaItem.MimeType);
            Assert.AreEqual("image", mediaItem.Type);
            Assert.AreEqual("http://animals.nationalgeographic.com/staticfiles/NGS/Shared/StaticFiles/animals/images/primary/black-spider-monkey.jpg", mediaItem.Url);
        }

        [Test]
        public void CanLoadMessages()
        {
            string path = @"canonicaldb.json";

            // act
            CanonicalDbLoader loader = new CanonicalDbLoader(path);
            var message = loader.MessageCollections;

            Assert.IsNotNull(message);
        }

        [Test]
        public void CanLoadGroups()
        {
            string path = @"canonicaldb.json";

            // act
            CanonicalDbLoader loader = new CanonicalDbLoader(path);
            var groups = loader.GroupCollections;

            Assert.IsNotNull(groups);
        }
    }
}
