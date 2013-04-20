// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OAuth2Manager.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Provides basic functionality to process oauth2 requests
//   Should be replaced with open libraries (dotnetopenauth)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Utils.OAuth
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// Provides basic functionality to process oauth2 requests
    /// Should be replaced with open libraries (dotnetopenauth)
    /// </summary>
    public class OAuth2Manager
    {
        /// <summary>
        /// Client id
        /// </summary>
        private readonly string _clientId;

        /// <summary>
        /// Client shared secret
        /// </summary>
        private readonly string _secret;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2Manager"/> class.
        /// </summary>
        /// <param name="clientId"> Client id. </param>
        /// <param name="secret"> Client shared secret. </param>
        public OAuth2Manager(string clientId, string secret)
        {
            this._clientId = clientId;
            this._secret = secret;
        }

        /// <summary>
        /// Gets oauth2 request url
        /// </summary>
        /// <param name="uri"> Request uri. </param>
        /// <param name="callback"> Callback uri. </param>
        /// <param name="scope"> Scope param. </param>
        /// <param name="state"> State param. </param>
        /// <returns> Valid oauth2 request uri </returns>
        public Uri GetRequestUri(Uri uri, string callback, string scope, string state)
        {
            return
                new Uri(string.Format(
                        "{0}?client_id={1}&redirect_uri={2}&response_type=code&scope={3}&state={4}",
                        uri,
                        this._clientId,
                        HttpUtility.UrlEncode(callback),
                        HttpUtility.UrlEncode(scope),
                        state));
        }

        /// <summary>
        /// Gets access token task
        /// </summary>
        /// <param name="uri"> Request uri. </param>
        /// <param name="method"> Http method. </param>
        /// <param name="callback"> Callback url. </param>
        /// <param name="code"> Verifier code. </param>
        /// <returns>Async task </returns>
        public Task<IDictionary<string, string>> AcquireAccessToken(Uri uri, string method, string callback, string code)
        {
            string @params =
                string.Format(
                    "client_id={0}&client_secret={1}&redirect_uri={2}&code={3}&grant_type=authorization_code",
                    this._clientId,
                    this._secret,
                    HttpUtility.UrlEncode(callback),
                    code); 
            HttpWebRequest request;
            Func<string, IDictionary<string, string>> responseConverter;
            switch (method)
            {
                case "GET":
                    var accessUri = new Uri(string.Format("{0}?{1}", uri, @params));
                    request = (HttpWebRequest)WebRequest.Create(accessUri);
                    request.Method = method;
                    responseConverter = this.GetDictionaryFromKeyValuePair;
                    break;
                case "POST":
                    request = (HttpWebRequest)WebRequest.Create(uri);
                    request.Method = method;
                    request.ContentType = "application/x-www-form-urlencoded";
                    responseConverter = JsonHelper.GetDictionary;
                    using (var writer = new StreamWriter(request.GetRequestStream()))
                    {
                        writer.Write(@params);
                    }

                    break;
                default:
                    throw new NotSupportedException(string.Format("http method: {0}", method));
            }

            return Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null, TaskCreationOptions.AttachedToParent)
                .ContinueWith(t =>
                {
                    HttpWebResponse response = null;
                    StreamReader reader = null;
                    try
                    {
                        response = (HttpWebResponse)t.Result;
                        reader = new StreamReader(response.GetResponseStream());
                        
                        // TODO get response analyzing content type
                        return responseConverter(reader.ReadToEnd());
                    }
                    finally
                    {
                        if (response != null)
                        {
                            response.Close();
                        }

                        if (reader != null)
                        {
                            reader.Close();
                        }
                    }
                });
        }

        /// <summary>
        /// Gets dictionary uri.
        /// </summary>
        /// <param name="content"> Content string. </param>
        /// <returns> Dictionary object</returns>
        private IDictionary<string, string> GetDictionaryFromKeyValuePair(string content)
        {
            var @params = new Dictionary<string, string>();
            var kvpairs = content.Split('&');
            foreach (var pair in kvpairs)
            {
                var kv = pair.Split('=');
                @params.Add(kv[0], kv[1]);
            }

            return @params;
        }
    }
}
