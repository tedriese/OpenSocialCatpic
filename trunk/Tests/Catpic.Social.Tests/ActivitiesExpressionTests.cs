using NUnit.Framework;

namespace Catpic.Social.Tests
{
    [TestFixture]
    public class ActivitiesExpressionTests
    {

        [TestCase(
       "[{\"method\":\"activities.get\",\"id\":\"activities.get\",\"params\":{\"userId\":\"john.doe\",\"groupId\":\"@self\"}}]",
       "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":1,\"filtered\":false,\"sorted\":false,\"updatedSince\":true,\"list\":[{\"id\":\"1\",\"userId\":\"john.doe\",\"body\":\"what a color!\",\"title\":\"yellow\"}]},\"id\":\"activities.get\"}]")]
        public void CanLoadSelf(string query, string expectedResult)
        {
            TestHelper.AsertResult(query, expectedResult);
        }


        [TestCase(
       "[{\"method\":\"activities.get\",\"id\":\"activities.get\",\"params\":{\"userId\":\"john.doe\",\"groupId\":\"@friends\"}}]",
       "[{\"result\":{\"startIndex\":0,\"itemsPerPage\":25,\"totalResults\":2,\"filtered\":false,\"sorted\":false,\"updatedSince\":true,\"list\":[{\"id\":\"1\",\"userId\":\"jane.doe\",\"body\":\"and she thinks you look like him\",\"streamTitle\":\"jane's photos\",\"title\":\"Jane just posted a photo of a monkey\",\"mediaItems\":[{\"mimeType\":\"image/jpeg\",\"type\":\"image\",\"url\":\"http://animals.nationalgeographic.com/staticfiles/NGS/Shared/StaticFiles/animals/images/primary/black-spider-monkey.jpg\"},{\"mimeType\":\"image/jpeg\",\"type\":\"image\",\"url\":\"http://image.guardian.co.uk/sys-images/Guardian/Pix/gallery/2002/01/03/monkey300.jpg\"}]},{\"id\":\"2\",\"userId\":\"jane.doe\",\"body\":\"or is it you?\",\"streamTitle\":\"jane's photos\",\"title\":\"Jane says George likes yoda!\",\"mediaItems\":[{\"mimeType\":\"image/jpeg\",\"type\":\"image\",\"url\":\"http://www.funnyphotos.net.au/images/fancy-dress-dog-yoda-from-star-wars1.jpg\"}]}]},\"id\":\"activities.get\"}]")]
        public void CanLoadFriends(string query, string expectedResult)
        {
            TestHelper.AsertResult(query, expectedResult);
        }
    }
}
