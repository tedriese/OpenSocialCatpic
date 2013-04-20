// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestProxy.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Handles calls from client side API
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Proxies
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Threading.Tasks;

    using Catpic.Utils;
    using Catpic.Utils.Diagnostic;

    using Newtonsoft.Json;

    /// <summary>
    /// Handles calls from client side API
    /// </summary>
    public class RequestProxy : IRequestProxy
    {
        /// <summary>
        /// Trace category
        /// </summary>
        private const string TraceCategory = "request.proxy";

        /// <summary>
        /// Trace instance
        /// </summary>
        private static readonly ITrace Trace = TraceFactory.GetTrace();

        /// <summary>
        /// Process gadgets.makeRequest
        /// </summary>
        /// <param name="context"> Proxy context. </param>
        /// <param name="headers"> Http headers map. </param>
        /// <param name="queryString"> Additional query String. </param>
        /// <returns> Async task. </returns>
        public Task MakeRequestAsync(ProxyContext context, IDictionary<string, string> headers, string queryString)
        {
            var request = context.Http.Request;
            NameValueCollection qs = request.HttpMethod == "GET" ? request.QueryString : request.Form;

            var originalUri = qs["url"];
            var requestUri = GetUri(originalUri, queryString);
            var method = qs["httpMethod"];

            return RemoteFetchHelper.GetFetchDataTask(requestUri, method, headers, null, TaskCreationOptions.None)
                .ContinueWith(t =>
                {
                    try
                    {
                        var response = t.Result;
                        var token = context.SecurityToken;
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            // TODO process response headers: encoding, datestamp, etc
                            var reader = new StreamReader(responseStream);
                            var strResponse = reader.ReadToEnd();
                            var jsonResponse =
                                JsonConvert.SerializeObject(
                                    new
                                        {
                                            rc = 200,
                                            st = token.ToClientState(),

                                            // TODO
                                            body = strResponse
                                        });

                            // NOTE:client-script will cut first 27 bytes
                            // NOTE response.ResponseUri may be different from request
                            context.Http.Response.Output.Write(string.Format("{0}{{\"{1}\":{2}}}", GadgetConsts.UnparseableCruft, originalUri, jsonResponse));
                        }
                    }
                    catch (AggregateException ex)
                    {
                        if (ex.InnerException != null)
                        {
                            Trace.Error(TraceCategory, string.Format("makeRequest: {0}", requestUri), ex.InnerException);
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.Error(TraceCategory, string.Format("makeRequest: {0}", requestUri), ex);
                    }
                });
        }

        /// <summary>
        /// Process proxy request
        /// </summary>
        /// <param name="context"> Proxy context. </param>
        /// <param name="headers"> Http headers map. </param>
        /// <param name="queryString"> Additional query String. </param>
        /// <returns> Async task. </returns>
        public Task ProxyRequestAsync(ProxyContext context, IDictionary<string, string> headers, string queryString)
        {
            var requestUri = new Uri(context.Http.Request["url"]);
            var method = context.Http.Request.HttpMethod;
            return RemoteFetchHelper.GetFetchDataTask(requestUri, method, null, TaskCreationOptions.None)
                .ContinueWith(t =>
                {
                    try
                    {
                        var response = t.Result;
                        string contentType = response.ContentType;
                        Stream content = response.GetResponseStream();
                        var memoryStream = new MemoryStream();
                        content.CopyTo(memoryStream);
                        context.Http.Response.ContentType = contentType;
                        context.Http.Response.BinaryWrite(memoryStream.ToArray());
                    }
                    catch (AggregateException ex)
                    {
                        if (ex.InnerException != null)
                        {
                            Trace.Error(TraceCategory, string.Format("proxy: {0}", requestUri), ex.InnerException);
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.Error(TraceCategory, string.Format("proxy: {0}", requestUri), ex);
                    }
                });
        }

        /// <summary>
        /// Builds uri from two base and relative uri
        /// </summary>
        /// <param name="original"> The original uri. </param>
        /// <param name="authQueryString"> The auth query string. </param>
        /// <returns> Result uri </returns>
        private static Uri GetUri(string original, string authQueryString)
        {
            if (string.IsNullOrEmpty(authQueryString))
            {
                return new Uri(original);
            }

            var symbol = "?";
            if (original.Contains("?"))
            {
                symbol = "&";
            }

            return new Uri(string.Format("{0}{1}{2}", original, symbol, authQueryString));
        }
    }
}
