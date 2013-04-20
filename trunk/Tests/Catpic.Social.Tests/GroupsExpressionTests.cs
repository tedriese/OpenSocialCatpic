using NUnit.Framework;

namespace Catpic.Social.Tests
{
    [TestFixture]
    public class GroupsExpressionTests
    {

        [TestCase(
       "[{\"method\":\"groups.get\",\"id\":\"groups.get\",\"params\":{\"userId\":\"john.doe\"}}]",
       "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":2,\"filtered\":false,\"sorted\":false,\"updatedSince\":true,\"list\":[{\"id\":\"example_com_391nvf03381\",\"title\":\"Group 2\",\"description\":\"A group of people\"},{\"id\":\"example_com_390e3kd03\",\"title\":\"Group 3\",\"description\":\"Another group of people\"}]},\"id\":\"groups.get\"}]")]
        public void CanLoadList(string query, string expectedResult)
        {
            TestHelper.AsertResult(query, expectedResult);
        }


        [TestCase(
       "[{\"method\":\"groups.get\",\"id\":\"groups.get\",\"params\":{\"userId\":\"john.doe\",\"groupId\":\"example_com_390e3kd03\"}}]",
       "[{\"result\":{\"id\":\"example_com_390e3kd03\",\"title\":\"Group 3\",\"description\":\"Another group of people\"},\"id\":\"groups.get\"}]")]
        public void CanLoadSingle(string query, string expectedResult)
        {
            TestHelper.AsertResult(query, expectedResult);
        }

        [TestCase(
      "[{\"method\":\"groups.create\",\"id\":\"groups.create\",\"params\":{\"userId\":\"@viewer\",\"group\":{\"title\":\"test title1\",\"test description\":\"\"}}}]",
      "[{\"result\":{\"id\":\"testID\",\"title\":\"test title1\"},\"id\":\"groups.create\"}]",
      "[{\"method\":\"groups.get\",\"id\":\"groups.get\",\"params\":{\"userId\":\"john.doe\"}}]",
      "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":3,\"filtered\":false,\"sorted\":false,\"updatedSince\":true,\"list\":[{\"id\":\"example_com_391nvf03381\",\"title\":\"Group 2\",\"description\":\"A group of people\"},{\"id\":\"example_com_390e3kd03\",\"title\":\"Group 3\",\"description\":\"Another group of people\"},{\"id\":\"testID\",\"title\":\"test title1\"}]},\"id\":\"groups.get\"}]")]
        public void CanCreate(string query1, string expectedResult1, string query2, string expectedResult2)
        {
            TestHelper.DoubleCheck(query1, expectedResult1, query2, expectedResult2);
        }

        [TestCase(
        "[{\"method\":\"groups.update\",\"id\":\"groups.update\",\"params\":{\"userId\":\"@viewer\",\"group\":{\"id\":\"example_com_391nvf03381\",\"title\":\"test title1\"}}}]",
        "[{\"result\":{\"id\":\"example_com_391nvf03381\",\"title\":\"test title1\",\"description\":\"A group of people\"},\"id\":\"groups.update\"}]",
        "[{\"method\":\"groups.get\",\"id\":\"groups.get\",\"params\":{\"userId\":\"john.doe\"}}]",
        "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":2,\"filtered\":false,\"sorted\":false,\"updatedSince\":true,\"list\":[{\"id\":\"example_com_391nvf03381\",\"title\":\"test title1\",\"description\":\"A group of people\"},{\"id\":\"example_com_390e3kd03\",\"title\":\"Group 3\",\"description\":\"Another group of people\"}]},\"id\":\"groups.get\"}]")]
        public void CanUpdate(string query1, string expectedResult1, string query2, string expectedResult2)
        {
            TestHelper.DoubleCheck(query1, expectedResult1, query2, expectedResult2);
        }

        [TestCase(
        "[{\"method\":\"groups.delete\",\"id\":\"groups.delete\",\"params\":{\"userId\":\"@viewer\",\"groupId\":\"example_com_391nvf03381\"}}]",
        "[{\"result\":{},\"id\":\"groups.delete\"}]",
        "[{\"method\":\"groups.get\",\"id\":\"groups.get\",\"params\":{\"userId\":\"john.doe\"}}]",
        "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":1,\"filtered\":false,\"sorted\":false,\"updatedSince\":true,\"list\":[{\"id\":\"example_com_390e3kd03\",\"title\":\"Group 3\",\"description\":\"Another group of people\"}]},\"id\":\"groups.get\"}]")]
        public void CanDelete(string query1, string expectedResult1, string query2, string expectedResult2)
        {
            TestHelper.DoubleCheck(query1, expectedResult1, query2, expectedResult2);
        } 
    }
}
