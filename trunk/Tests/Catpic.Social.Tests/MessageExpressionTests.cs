// -----------------------------------------------------------------------
// <copyright file="MessageExpressionTests.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Catpic.Social.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    using Catpic.Host.Engine.Social;
    using Catpic.Social.Formatting;
    using Catpic.Social.Messages;
    using Catpic.Utils.Linq;

    using NUnit.Framework;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [TestFixture]
    public class MessageExpressionTests
    {
        #region Message tests

        [TestCase(
             "[{\"method\":\"messages.get\",\"id\":\"singleMessage\",\"params\":{\"userId\":\"john.doe\",\"messageCollectionId\":\"notification\",\"messageId\":\"1\"}}]",
             "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":1,\"filtered\":true,\"sorted\":false,\"updatedSince\":true,\"list\":[{\"id\":\"1\",\"type\":\"notification\",\"body\":\"peanuts are healthy\",\"title\":\"you received a peanut\"}]},\"id\":\"singleMessage\"}]")]
        public void CanGetSingleMessage(string query, string expectedResult)
        {
            TestHelper.AsertResult(query, expectedResult);
        }

        [TestCase(
            "[{\"method\":\"messages.get\",\"id\":\"singleCollection\",\"params\":{\"userId\":\"john.doe\",\"messageCollectionId\":\"notification\"}}]",
            "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":2,\"filtered\":false,\"sorted\":false,\"updatedSince\":true,\"list\":[{\"id\":\"1\",\"type\":\"notification\",\"body\":\"peanuts are healthy\",\"title\":\"you received a peanut\"},{\"id\":\"3\",\"type\":\"notification\",\"body\":\"Join Cat Lovers Anonymous\",\"title\":\"Group Request\"}]},\"id\":\"singleCollection\"}]")]
        public void CanGetCollectionMessages(string query, string expectedResult)
        {
            TestHelper.AsertResult(query, expectedResult);
        }

        [TestCase(
         "[{\"method\":\"messages.send\",\"id\":\"messages.send\",\"params\":{\"userId\":\"john.doe\", \"message\":{\"type\":\"privateMessage\",\"collectionIds\":[\"privateMessage\"],\"title\":\"Message from UnitTest\",\"body\":\"Body of email\",\"recipients\":[\"jane.doe\"]}}}]",
         "[{\"result\":{\"type\":\"privateMessage\",\"body\":\"Body of email\",\"title\":\"Message from UnitTest\",\"recipients\":[\"jane.doe\"],\"senderId\":\"john.doe\",\"timeSent\":\"9/8/2012 11:17:16 PM\",\"collectionIds\":[\"privateMessage\"],\"updated\":\"9/8/2012 11:17:16 PM\"},\"id\":\"messages.send\"}]",
         "[{\"method\":\"messages.get\",\"id\":\"messages.get\",\"params\":{\"userId\":\"jane.doe\", \"messageCollectionId\":\"privateMessage\"}}]",
         "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":1,\"filtered\":false,\"sorted\":false,\"updatedSince\":true,\"list\":[{\"type\":\"privateMessage\",\"body\":\"Body of email\",\"title\":\"Message from UnitTest\",\"recipients\":[\"jane.doe\"],\"senderId\":\"john.doe\",\"timeSent\":\"9/8/2012 11:17:16 PM\",\"collectionIds\":[\"privateMessage\"],\"updated\":\"9/8/2012 11:17:16 PM\"}]},\"id\":\"messages.get\"}]")]
        public void CanSendMessage(string query1, string expectedResult1, string query2, string expectedResult2)
        {
            TestHelper.DoubleCheck(query1, expectedResult1, query2, expectedResult2);
        }

        [TestCase(
         "[{\"method\":\"messages.delete\",\"id\":\"messages.delete\",\"params\":{\"userId\":\"john.doe\", \"messageCollectionId\":\"notification\",\"messageId\":\"1\"}}]",
         "[{\"result\":{\"id\":\"1\"},\"id\":\"messages.delete\"}]",
         "[{\"method\":\"messages.get\",\"id\":\"messages.get\",\"params\":{\"userId\":\"john.doe\", \"messageCollectionId\":\"notification\"}}]",
         "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":1,\"filtered\":false,\"sorted\":false,\"updatedSince\":true,\"list\":[{\"id\":\"3\",\"type\":\"notification\",\"body\":\"Join Cat Lovers Anonymous\",\"title\":\"Group Request\"}]},\"id\":\"messages.get\"}]")]
        public void CanDeleteMessage(string query1, string expectedResult1, string query2, string expectedResult2)
        {
            TestHelper.DoubleCheck(query1, expectedResult1, query2, expectedResult2);
        }

        [TestCase(
         "[{\"method\":\"messages.update\",\"id\":\"messages.update\",\"params\":{\"userId\":\"john.doe\",\"messageCollectionId\":\"notification\",\"message\":{\"id\":\"1\",\"type\":\"notification\",\"body\":\"1peanuts are healthy1\",\"title\":\"1you received a peanut1\"}}}]",
         "[{\"result\":{\"id\":\"1\",\"type\":\"notification\",\"body\":\"1peanuts are healthy1\",\"title\":\"1you received a peanut1\"},\"id\":\"messages.update\"}]",
         "[{\"method\":\"messages.get\",\"id\":\"messages.get\",\"params\":{\"userId\":\"john.doe\", \"messageCollectionId\":\"notification\",\"sortBy\":\"id\",\"sortOrder\":\"ascending\"}}]",
         "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":2,\"filtered\":false,\"sorted\":true,\"updatedSince\":true,\"list\":[{\"id\":\"1\",\"type\":\"notification\",\"body\":\"1peanuts are healthy1\",\"title\":\"1you received a peanut1\"},{\"id\":\"3\",\"type\":\"notification\",\"body\":\"Join Cat Lovers Anonymous\",\"title\":\"Group Request\"}]},\"id\":\"messages.get\"}]")]
        public void CanUpdateMessage(string query1, string expectedResult1, string query2, string expectedResult2)
        {
            TestHelper.DoubleCheck(query1, expectedResult1, query2, expectedResult2);
        }

        #endregion

        #region Message collection tests

        [TestCase(
           "[{\"method\":\"messages.get\",\"id\":\"messages.get\",\"params\":{\"userId\":\"john.doe\"}}]",
           "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":3,\"filtered\":false,\"sorted\":false,\"updatedSince\":true,\"list\":[{\"id\":\"notification\",\"title\":\"Notifications\"},{\"id\":\"privateMessage\",\"title\":\"Private Inbox\"},{\"id\":\"publicMessage\",\"title\":\"Profile Comments\"}]},\"id\":\"messages.get\"}]")]
        public void CanGetCollectionList(string query, string expectedResult)
        {
            TestHelper.AsertResult(query, expectedResult);
        }

        [TestCase(
          "[{\"method\":\"messages.create\",\"id\":\"messages.create\",\"params\":{\"userId\":\"john.doe\",\"name\": \"test collection\", \"messageCollectionId\":\"testCollectionId\" }}]",
          "[{\"result\":{\"id\":\"testCollectionId\"},\"id\":\"messages.create\"}]",
          "[{\"method\":\"messages.get\",\"id\":\"messages.get\",\"params\":{\"userId\":\"john.doe\"}}]",
          "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":4,\"filtered\":false,\"sorted\":false,\"updatedSince\":true,\"list\":[{\"id\":\"notification\",\"title\":\"Notifications\"},{\"id\":\"privateMessage\",\"title\":\"Private Inbox\"},{\"id\":\"publicMessage\",\"title\":\"Profile Comments\"},{\"id\":\"testCollectionId\",\"title\":\"test collection\"}]},\"id\":\"messages.get\"}]")]
        public void CanCreateCollection(string query1, string expectedResult1, string query2, string expectedResult2)
        {
            TestHelper.DoubleCheck(query1, expectedResult1, query2, expectedResult2);
        }

        [TestCase(
         "[{\"method\":\"messages.delete\",\"id\":\"messages.delete\",\"params\":{\"userId\":\"john.doe\",\"messageCollectionId\":\"notification\"}}]",
         "[{\"result\":{\"id\":\"notification\"},\"id\":\"messages.delete\"}]",
         "[{\"method\":\"messages.get\",\"id\":\"messages.get\",\"params\":{\"userId\":\"john.doe\"}}]",
         "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":2,\"filtered\":false,\"sorted\":false,\"updatedSince\":true,\"list\":[{\"id\":\"privateMessage\",\"title\":\"Private Inbox\"},{\"id\":\"publicMessage\",\"title\":\"Profile Comments\"}]},\"id\":\"messages.get\"}]")]
        public void CanDeleteCollection(string query1, string expectedResult1, string query2, string expectedResult2)
        {
            TestHelper.DoubleCheck(query1, expectedResult1, query2, expectedResult2);
        }

        [TestCase(
         "[{\"method\":\"messages.update\",\"id\":\"messages.update\",\"params\":{\"userId\":\"john.doe\", \"name\":\"myNewNotifications\",\"messageCollectionId\":\"notification\"}}]",
         "[{\"result\":\"myNewNotifications\",\"id\":\"messages.update\"}]",
         "[{\"method\":\"messages.get\",\"id\":\"messages.get\",\"params\":{\"userId\":\"john.doe\"}}]",
         "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":3,\"filtered\":false,\"sorted\":false,\"updatedSince\":true,\"list\":[{\"id\":\"notification\",\"title\":\"myNewNotifications\"},{\"id\":\"privateMessage\",\"title\":\"Private Inbox\"},{\"id\":\"publicMessage\",\"title\":\"Profile Comments\"}]},\"id\":\"messages.get\"}]")]
        public void CanUpdateCollection(string query1, string expectedResult1, string query2, string expectedResult2)
        {
            TestHelper.DoubleCheck(query1, expectedResult1, query2, expectedResult2);
        }

        #endregion
    }
}
