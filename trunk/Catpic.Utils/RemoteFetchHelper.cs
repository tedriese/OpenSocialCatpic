// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteFetchHelper.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Defines the RemoteFetchHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides webRequest extension methods
    /// </summary>
    public static class RemoteFetchHelper
    {
        /// <summary>
        /// Sync fetch data
        /// </summary>
        /// <param name="url"> Target url. </param>
        /// <param name="method"> Http method. </param>
        /// <returns> Response stream </returns>
        public static Stream FetchData(string url, string method)
        {
            var request = WebRequest.Create(url);
            request.Method = method;
            var stream = request.GetResponse();

            // TODO assert response here
            return stream.GetResponseStream();
        }

        /// <summary>
        /// Gets async task which fetchs data from remote resource
        /// </summary>
        /// <param name="requestUri"> Target request uri. </param>
        /// <param name="method"> Http method. </param>
        /// <param name="asyncState"> Async state. </param>
        /// <param name="taskOptions"> Async task options. </param>
        /// <returns> Async task</returns>
        public static Task<WebResponse> GetFetchDataTask(Uri requestUri, string method, object asyncState, TaskCreationOptions taskOptions)
        {
            return GetFetchDataTask(requestUri, method, asyncState, taskOptions, Task.Factory);
        }

        /// <summary>
        /// Gets async task which fetchs data from remote resource
        /// </summary>
        /// <param name="requestUri"> Target request uri. </param>
        /// <param name="method"> Http method. </param>
        /// <param name="asyncState"> Async state. </param>
        /// <param name="taskOptions"> Async task options. </param>
        /// <param name="tf"> Task factory. </param>
        /// <returns> Async task</returns>
        public static Task<WebResponse> GetFetchDataTask(Uri requestUri, string method, object asyncState, TaskCreationOptions taskOptions, TaskFactory tf)
        {
            return GetFetchDataTask(requestUri, method, null, asyncState, taskOptions, tf);
        }

        /// <summary>
        /// Gets async task which fetchs data from remote resource
        /// </summary>
        /// <param name="requestUri"> Target request uri. </param>
        /// <param name="method"> Http method. </param>
        /// <param name="headers">Request headers</param>
        /// <param name="asyncState"> Async state. </param>
        /// <param name="taskOptions"> Async task options. </param>
        /// <returns>Async task</returns>
        public static Task<WebResponse> GetFetchDataTask(Uri requestUri, string method, IDictionary<string, string> headers, object asyncState, TaskCreationOptions taskOptions)
        {
            return GetFetchDataTask(requestUri, method, headers, asyncState, taskOptions, Task.Factory);
        }

        /// <summary>
        /// Gets async task which fetchs data from remote resource
        /// </summary>
        /// <param name="requestUri"> Target request uri. </param>
        /// <param name="method"> Http method. </param>
        /// <param name="headers">Request headers</param>
        /// <param name="asyncState"> Async state. </param>
        /// <param name="taskOptions"> Async task options. </param>
        /// <param name="tf"> Task factory. </param>
        /// <returns>Async task</returns>
        public static Task<WebResponse> GetFetchDataTask(
            Uri requestUri,
            string method,
            IDictionary<string, string> headers,
            object asyncState,
            TaskCreationOptions taskOptions,
            TaskFactory tf)
        {
            var proxyRequest = WebRequest.Create(requestUri);
            if (headers != null)
            {
                foreach (var key in headers.Keys)
                {
                    proxyRequest.Headers.Add(key, headers[key]);    
                }
            }
            
            proxyRequest.Method = method;
            return tf.FromAsync<WebResponse>(proxyRequest.BeginGetResponse, proxyRequest.EndGetResponse, asyncState, taskOptions);
        }
    }
}
