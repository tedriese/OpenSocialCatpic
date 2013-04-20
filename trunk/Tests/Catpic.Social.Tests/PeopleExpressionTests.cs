using NUnit.Framework;

namespace Catpic.Social.Tests
{
    [TestFixture]
    public class PeopleExpressionTests
    {

        #region Select methods

        [TestCase(
            "[{\"method\":\"people.get\",\"params\":{\"userId\":\"@viewer\",\"groupId\":\"@friends\",\"networkDistance\":1,\"filterBy\":\"id\",\"filterValue\":\"jane.doe\",\"filterOp\": \"equals\",\"sortBy\":\"thumbnailUrl\",\"sortOrder\":\"ascending\", \"fields\":[\"id\",\"displayName\",\"thumbnailUrl\"]}}]",
            "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":1,\"filtered\":true,\"sorted\":true,\"updatedSince\":true,\"list\":[{\"id\":\"jane.doe\",\"displayName\":\"Janey\",\"thumbnailUrl\":\"/Content/Social/Avatars/jane.doe.jpg\"}]},\"id\":\"\"}]")]
        [TestCase(
        "[{\"method\":\"people.get\",\"params\":{\"userId\":\"@viewer\",\"groupId\":\"@friends\",\"networkDistance\":1,\"sortBy\":\"displayName\",\"sortOrder\":\"ascending\", \"fields\":[\"id\",\"displayName\",\"thumbnailUrl\"]}}]",
        "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":3,\"filtered\":false,\"sorted\":true,\"updatedSince\":true,\"list\":[{\"id\":\"george.doe\",\"displayName\":\"Georgey\",\"thumbnailUrl\":\"/Content/Social/Avatars/george.doe.jpg\"},{\"id\":\"jane.doe\",\"displayName\":\"Janey\",\"thumbnailUrl\":\"/Content/Social/Avatars/jane.doe.jpg\"},{\"id\":\"maija.m\",\"displayName\":\"Maija\",\"thumbnailUrl\":\"/Content/Social/Avatars/maija.m.jpg\"}]},\"id\":\"\"}]")]
        public void CanGetViewerFriends(string query, string expectedResult)
        {
            TestHelper.AsertResult(query, expectedResult);
        }

        [TestCase(
            "[{\"method\":\"people.get\",\"params\":{\"userId\":\"@viewer\",\"groupId\":\"@self\",\"fields\":[\"id\",\"displayName\"]},\"id\":\"get_viewer\"}]",
            "[{\"result\":{\"id\":\"john.doe\",\"displayName\":\"Johnny\"},\"id\":\"get_viewer\"}]")]
        public void CanGetViewerSelf(string query, string expectedResult)
        {
            TestHelper.AsertResult(query, expectedResult);
        }

        [TestCase(
        "[{\"method\":\"people.get\",\"params\":{\"userId\":\"@viewer\",\"groupId\":\"@friends\",\"networkDistance\":1,\"sortBy\":\"displayName\",\"sortOrder\":\"descending\", \"fields\":[\"id\",\"displayName\"]}}]",
        "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":3,\"filtered\":false,\"sorted\":true,\"updatedSince\":true,\"list\":[{\"id\":\"maija.m\",\"displayName\":\"Maija\"},{\"id\":\"jane.doe\",\"displayName\":\"Janey\"},{\"id\":\"george.doe\",\"displayName\":\"Georgey\"}]},\"id\":\"\"}]")]
         [TestCase(
        "[{\"method\":\"people.get\",\"params\":{\"userId\":\"@viewer\",\"groupId\":\"@friends\",\"networkDistance\":1,\"sortBy\":\"displayName\",\"sortOrder\":\"ascending\", \"fields\":[\"id\",\"displayName\"]}}]",
        "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":3,\"filtered\":false,\"sorted\":true,\"updatedSince\":true,\"list\":[{\"id\":\"george.doe\",\"displayName\":\"Georgey\"},{\"id\":\"jane.doe\",\"displayName\":\"Janey\"},{\"id\":\"maija.m\",\"displayName\":\"Maija\"}]},\"id\":\"\"}]")]
        public void CanSortViewerFriends(string query, string expectedResult)
        {
            TestHelper.AsertResult(query, expectedResult);
        }

         [TestCase(
        "[{\"method\":\"people.get\",\"params\":{\"userId\":\"@viewer\",\"groupId\":\"@friends\",\"networkDistance\":1,\"sortBy\":\"id\",\"filterBy\":\"id\",\"filterValue\":\"doe\",\"filterOp\": \"contains\",\"sortOrder\":\"ascending\", \"fields\":[\"id\",\"displayName\"]}}]",
        "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":2,\"filtered\":true,\"sorted\":true,\"updatedSince\":true,\"list\":[{\"id\":\"george.doe\",\"displayName\":\"Georgey\"},{\"id\":\"jane.doe\",\"displayName\":\"Janey\"}]},\"id\":\"\"}]")]
         [TestCase(
        "[{\"method\":\"people.get\",\"params\":{\"userId\":\"@viewer\",\"groupId\":\"@friends\",\"networkDistance\":1,\"filterBy\":\"id\",\"filterOp\": \"contains\", \"filterValue\":\"doe\",\"sortBy\":\"id\",\"sortOrder\":\"ascending\", \"fields\":[\"id\",\"displayName\"]}}]",
        "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":2,\"filtered\":true,\"sorted\":true,\"updatedSince\":true,\"list\":[{\"id\":\"george.doe\",\"displayName\":\"Georgey\"},{\"id\":\"jane.doe\",\"displayName\":\"Janey\"}]},\"id\":\"\"}]")]
         [TestCase(
        "[{\"method\":\"people.get\",\"params\":{\"userId\":\"@viewer\",\"groupId\":\"@friends\",\"networkDistance\":1,\"filterBy\":\"id\",\"filterOp\": \"startsWith\", \"filterValue\":\"ge\",\"sortBy\":\"id\",\"sortOrder\":\"ascending\", \"fields\":[\"id\",\"displayName\"]}}]",
        "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":1,\"filtered\":true,\"sorted\":true,\"updatedSince\":true,\"list\":[{\"id\":\"george.doe\",\"displayName\":\"Georgey\"}]},\"id\":\"\"}]")]
         public void CanFilterViewerFriends(string query, string expectedResult)
         {
             TestHelper.AsertResult(query, expectedResult);
         }

         [TestCase(
        "[{\"method\":\"people.get\",\"params\":{\"userId\":[\"@viewer\"],\"groupId\":\"@friends\",\"networkDistance\":1,\"sortBy\":\"id\",\"filterBy\":\"id\",\"filterValue\":\"doe\",\"filterOp\": \"contains\",\"sortOrder\":\"ascending\", \"fields\":[\"id\",\"displayName\"]}}]",
        "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":2,\"filtered\":true,\"sorted\":true,\"updatedSince\":true,\"list\":[{\"id\":\"george.doe\",\"displayName\":\"Georgey\"},{\"id\":\"jane.doe\",\"displayName\":\"Janey\"}]},\"id\":\"\"}]")]
          public void CanProcessUserIdArray(string query, string expectedResult)
          {
              TestHelper.AsertResult(query, expectedResult);
          }

         [TestCase(
        "[{\"method\":\"people.get\",\"id\":\"people.get\",\"params\":\"bad_param\"}]",
        "[{\"error\":{\"code\":500,\"message\":\"Unable to validate people.get: Object reference not set to an instance of an object.\"},\"id\":\"people.get\"}]")]
        public void CanHandleErrorRequest(string query, string expectedResult)
        {
            TestHelper.AsertResult(query, expectedResult);
        }

        [TestCase("[{\"method\":\"people.get\",\"id\":\"people.get\",\"params\":{\"networkDistance\":1,\"startIndex\":0,\"count\":1,\"userId\":\"@viewer\",\"groupId\":\"@friends\",\"fields\":[\"id\",\"displayName\"]}}]",
        "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":1,\"totalResults\":1,\"filtered\":false,\"sorted\":false,\"updatedSince\":true,\"list\":[{\"id\":\"jane.doe\",\"displayName\":\"Janey\"}]},\"id\":\"people.get\"}]")]
        [TestCase("[{\"method\":\"people.get\",\"id\":\"people.get\",\"params\":{\"networkDistance\":1,\"startIndex\":0,\"count\":2,\"userId\":\"@viewer\",\"groupId\":\"@friends\",\"fields\":[\"id\",\"displayName\"]}}]",
        "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":2,\"totalResults\":2,\"filtered\":false,\"sorted\":false,\"updatedSince\":true,\"list\":[{\"id\":\"jane.doe\",\"displayName\":\"Janey\"},{\"id\":\"george.doe\",\"displayName\":\"Georgey\"}]},\"id\":\"people.get\"}]")]
        [TestCase("[{\"method\":\"people.get\",\"id\":\"people.get\",\"params\":{\"networkDistance\":1,\"startIndex\":1,\"count\":1,\"userId\":\"@viewer\",\"groupId\":\"@friends\",\"fields\":[\"id\",\"displayName\"]}}]",
        "[{\"result\":{\"startIndex\":1,\"itemsPerPage\":1,\"totalResults\":1,\"filtered\":false,\"sorted\":false,\"updatedSince\":true,\"list\":[{\"id\":\"george.doe\",\"displayName\":\"Georgey\"}]},\"id\":\"people.get\"}]")]
        public void  CanGetPagedFriendList(string query, string expectedResult)
        {
            TestHelper.AsertResult(query, expectedResult);
        }

        [TestCase("{\"method\":\"people.create\",\"id\":\"createFriend\",\"params\":{\"userId\":\"@viewer\",\"groupId\":\"@friends\",\"person\":{\"id\":\"canonical\"}}}",
        "[{\"result\":{},\"id\":\"createFriend\"}]",
        "[{\"method\":\"people.get\",\"params\":{\"userId\":\"@viewer\",\"groupId\":\"@friends\",\"networkDistance\":1,\"filterBy\":\"id\",\"filterValue\":\"canonical\",\"filterOp\": \"equals\",\"fields\":[\"id\",\"displayName\"]}}]",
       "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":1,\"filtered\":true,\"sorted\":false,\"updatedSince\":true,\"list\":[{\"id\":\"canonical\",\"displayName\":\"Shin Digg\"}]},\"id\":\"\"}]")]
        public void CanCreateFriend(string query1, string expectedResult1, string query2, string expectedResult2)
        {
            TestHelper.DoubleCheck(query1, expectedResult1, query2, expectedResult2);
        }

        #endregion

        #region  Update, Delete

        [TestCase("{\"method\":\"people.delete\",\"id\":\"people.delete\",\"params\":{\"userId\":\"@viewer\",\"groupId\":\"@friends\",\"person\":{\"id\":\"jane.doe\"}}}",
         "[{\"result\":{},\"id\":\"people.delete\"}]",
         "[{\"method\":\"people.get\",\"params\":{\"userId\":\"@viewer\",\"groupId\":\"@friends\",\"fields\":[\"id\",\"displayName\"]}}]",
         "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":2,\"filtered\":false,\"sorted\":false,\"updatedSince\":true,\"list\":[{\"id\":\"george.doe\",\"displayName\":\"Georgey\"},{\"id\":\"maija.m\",\"displayName\":\"Maija\"}]},\"id\":\"\"}]")]
        public void CanDeleteFriend(string query1, string expectedResult1, string query2, string expectedResult2)
        {
            TestHelper.DoubleCheck(query1, expectedResult1, query2, expectedResult2);
        }

        [TestCase("{\"method\":\"people.update\",\"id\":\"people.update\",\"params\":{\"userId\":\"@viewer\",\"groupId\":\"@self\",\"person\":{\"id\":\"john.doe\",\"displayName\":\"Test1\"}}}",
        "[{\"result\":{\"id\":\"john.doe\",\"name\":{\"givenName\":\"John\",\"familyName\":\"Doe\",\"formatted\":\"John Doe\"},\"gender\":\"male\",\"displayName\":\"Test1\",\"thumbnailUrl\":\"/Content/Social/Avatars/john.doe.jpg\"},\"id\":\"people.update\"}]",
        "[{\"method\":\"people.get\",\"params\":{\"userId\":\"@viewer\",\"groupId\":\"@self\",\"fields\":[\"id\",\"displayName\"]}}]",
        "[{\"result\":{\"id\":\"john.doe\",\"displayName\":\"Test1\"},\"id\":\"\"}]")]
        public void CanUpdatePerson(string query1, string expectedResult1, string query2, string expectedResult2)
        {
            TestHelper.DoubleCheck(query1, expectedResult1, query2, expectedResult2);
        }

        #endregion
    }
}
