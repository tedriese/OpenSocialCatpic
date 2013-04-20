using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using Catpic.Utils.OAuth;

namespace Catpic.Utils.Tests
{
    [TestFixture]
    public class OAuthManagerTests
    {
        //NOTE commented out as environment specific
        [Test(Description = "tries to get access to http://term.ie/oauth/example/echo_api.php which echoes query string. See details here http://term.ie/oauth/example/index.php")]
        public void CanUseExternalService()
        {
            //http://term.ie/oauth/example/index.php
            string requestTokenUri = "http://term.ie/oauth/example/request_token.php";
            string accessTokenUri = "http://term.ie/oauth/example/access_token.php";
            string resource = "http://term.ie/oauth/example/echo_api.php?test=available&data=mydata";
            string consumerKey = "key";
            string consumerSecret = "secret";

            OAuthManager oauth = new OAuthManager();
            oauth["consumer_key"] = consumerKey;
            oauth["consumer_secret"] = consumerSecret;
            //get request token
            var task = oauth.AcquireRequestToken(requestTokenUri, "GET")
                .ContinueWith(t =>
                    {
                        //get access token
                        var requestResponse = t.Result;
                        Assert.AreEqual("requestkey", requestResponse["oauth_token"]);
                        Assert.AreEqual("requestsecret", requestResponse["oauth_token_secret"]);
                        return oauth.AcquireAccessToken(accessTokenUri, "GET", "");
                    }).Unwrap();
            task.Wait();
            //validate access token
            var accessResponse = task.Result;
            Assert.AreEqual("accesskey", accessResponse["oauth_token"]);
            Assert.AreEqual("accesssecret", accessResponse["oauth_token_secret"]);

            //try to get data using access token
            var authzHeader = oauth.GenerateAuthzHeader(resource, "GET");
            var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(resource);
            request.Headers.Add("Authorization", authzHeader);
            request.Method = "GET";

            using (var response = (System.Net.HttpWebResponse)request.GetResponse())
            {
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    Assert.AreEqual("test=available&data=mydata", reader.ReadToEnd());
                }
            }
        }
    }
}
